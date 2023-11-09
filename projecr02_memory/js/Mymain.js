$(document).ready(function () {
    var document = window.document

    //参数和变量
    var MEMORY_BLOCKS = 4 //共4块内存块
    var INSTRUCTIONS = 320 //共320条指令
    var PAGE_OF_INSTRUCTION = 10 //每页存放10条指令
    var PAGES = 32  //共32页

    var memory = new Array(MEMORY_BLOCKS) // 内存
    var count_instuctions // 记录执行的指令个数
    var mpage_num_of_FIFO // FIFO缺页数
    var mpage_num_of_LRU // LRU缺页数
    var record = new Array(PAGES) //记录最后一次调入内存的时间（用于LRU）

    // js和html的挂接通信
    var block1 = document.getElementById("block1")//内存块1
    var block2 = document.getElementById("block2")//内存块1
    var block3 = document.getElementById("block3")//内存块1
    var block4 = document.getElementById("block4")//内存块1
    var mpage_FIFO = document.getElementById("mpage_FIFO") //FIFO缺页数
    var mrate_FIFO = document.getElementById("mrate_FIFO") //FIFO缺页率
    var mpage_LRU = document.getElementById("mpage_LRU") //LRU缺页数
    var mrate_LRU = document.getElementById("mrate_LRU") //LRU缺页率
    var loading_bar = document.getElementById("loading_bar")//进度条 
    var now_page = document.getElementById("now_page")//当前执行页数
    var now_rate = document.getElementById("now_rate")
    $('#range_bar').on('input', function() {//添加对拖动条事件的监听
        var value = $(this).val();
        console.log('拖动条的值:', value);
        now_rate.innerHTML = value;
      });
    var start_btn = document.getElementById("start_btn")//开始按钮

    //延时输出
    function sleep(time){
        return new Promise((resolve) => setTimeout(resolve, time));
    }
    //用于判断指令是否已经在内存中，若在则返回其所在的内存块号，否则返回-1
    function in_memory(cur_ins) {
        let j
        for (let i = 0; i < memory.length; ++i) {
            j = Math.floor(cur_ins / PAGE_OF_INSTRUCTION)//除10得到该指令所在页号
            if (j === memory[i]) {
                return i //找到，返回内存块号
            }
        }
        return -1 //没找到，返回-1
    }


    //未满：返回第一个空内存块的序号，已满：返回-1
    function mem_not_full() {
        for(let i = 0; i < memory.length; i++){
            if(memory[i] === undefined){
                return i;
            }
        }
        return -1; 
    }
    //找放进哪个内存块
    function find_block(){
        var r = []
        for(let i =0;i<4;i++){
            r[i] = record[memory[i]]
        }
        var min_pior = Math.min(r[0],r[1],r[2],r[3])

        for(let j = 0; j < MEMORY_BLOCKS; j++){//找到是哪块应该被置换
            if(record[memory[j]] === min_pior){
                return j //第j块
            }
        }
    }
    //算法的选择
    function algorithm() {
        var choose = document.querySelector("input:checked")
        if (choose.value === "FIFO") {
            algo_FIFO()
        }
        else if (choose.value === "LRU") {
            algo_LRU()
        }
    }
    //每次算法开始前初始化内存块和指令数
    function init() {
        memory = new Array(MEMORY_BLOCKS) //内存块
        count_instuctions = 0 // 记录执行的指令个数
    }
    //FIFO原则
    async function algo_FIFO(){
        mpage_num_of_FIFO = 0 // 缺页个数，每次调用该变量都要置零
        var pointer = 0 //指向内存中最先进入内存的页面
        var cur_ins = -1  //当前指令
        var missing_flag = 0 //发生缺页置1
        var not_full_num
        var direction = 2
        /*direction作为指令跳转方向有四种情况：
          2 :顺序执行下一条指令，且上一步是向后跳转
          -1:跳转到前地址部分0－m-1中的某个指令处
          -2 :顺序执行下一条指令，且上一步是向前跳转
          1 :跳转到后地址部分m1+2~319中的某条指令处*/

        //第一条指令的地址
        //Math.random() 返回一个介于 0（包含）和 1（不包含）之间的随机浮点数。
        //这意味着它可以生成像 0、0.234、0.753 或 0.999 这样的值，但绝不会是完全等于 1 的值。
        //(INSTRUCTIONS - 1) 从 INSTRUCTIONS 的值中减去 1。得到的数确定了随机数的上限范围。
        // 将 Math.random() 乘以 (INSTRUCTIONS - 1)，
        //将 Math.random() 生成的随机数按比例缩放到 0 到 (INSTRUCTIONS - 1) 的范围内。
        //结果将是一个介于 0 和 (INSTRUCTIONS - 1) 之间的随机浮点数。
        // Math.round() 函数会将结果四舍五入为最接近的整数。
        cur_ins = Math.round( Math.random() * (INSTRUCTIONS-1))
        while( count_instuctions < INSTRUCTIONS ){
            var currentIns = cur_ins  //记录本次循环执行的指令
            missing_flag = 0
            await sleep(now_rate.innerHTML)//睡眠
            //改变跳转方向和下一条指令
            if(direction === 2){//顺序执行指令
                cur_ins ++  //顺序下一条
                direction = -1//往前跳
            }
            else if(direction === -1){
                //向前跳转
                if (cur_ins < 2) {
                    continue
                }
                //Math.floor会向下取整
                cur_ins = Math.floor(Math.random() * (cur_ins - 1))
                //切换至顺序执行
                direction = -2
            }
            else if(direction === -2){//顺序执行指令，且上一步是向后跳转
                cur_ins ++
                direction = 1//往前条
            }
            else if(direction === 1){//往跳后
                if (cur_ins > INSTRUCTIONS - 2) {
                    continue
                }
                cur_ins = Math.floor( Math.random() * (INSTRUCTIONS - (cur_ins + 2)) + (cur_ins + 2) )
                direction = 2 //顺序执行且上一步往后跳
            }

            //越界处理，增加健壮性
            if (cur_ins < 0) {
                cur_ins = -1
                direction = 1//往后跳
                continue
            }
            else if (cur_ins >= INSTRUCTIONS) {
                cur_ins = INSTRUCTIONS + 1
                direction = -1//往前跳
                continue
            }
            

            if(in_memory(currentIns) === -1){//缺页
                missing_flag = 1
                if(mem_not_full() !== -1){//内存未满
                    not_full_num = mem_not_full()
                    memory[not_full_num] = Math.floor(currentIns / PAGE_OF_INSTRUCTION)
                }
                else{//内存已满
                    memory[pointer] = Math.floor(currentIns / PAGE_OF_INSTRUCTION)
                    pointer = ( ++pointer ) % 4
                    mpage_FIFO.textContent = mpage_num_of_FIFO
                    mrate_FIFO.textContent = mpage_num_of_FIFO / INSTRUCTIONS
                }
                mpage_num_of_FIFO++ 
            }
            count_instuctions++
            loading_bar.value = count_instuctions//拖动条值更新
            now_page.innerHTML = count_instuctions//进度更新
            //界面显示调整
            var row = document.getElementById("memory_table").insertRow()
            row.insertCell(0).innerHTML = count_instuctions
            row.insertCell(1).innerHTML ="第" + currentIns + "条"
            for (let i = 0;i < 4;i++){
                row.insertCell(i + 2).innerHTML = 
                memory[i] == undefined ? "--" : memory[i]
            }
            //内存块的值导到html中
            block1.innerHTML = memory[0]
            block2.innerHTML = memory[1]
            block3.innerHTML = memory[2]
            block4.innerHTML = memory[3]
            if(missing_flag === 1){
                if(not_full_num !== -1){
                    row.insertCell(6).innerHTML = "缺页"
                    row.insertCell(7).innerHTML = " 调入第" + ((not_full_num)+1) + "个内存块"
                }
                else{
                    row.insertCell(6).innerHTML = "缺页 "
                    row.insertCell(7).innerHTML = " 调入第" + ((pointer-1 >= 0 ? pointer -1 : 3)+1) + "个内存块"
                }
            }
            else{
                row.insertCell(6).innerHTML = "不缺页" 
                row.insertCell(7).innerHTML = "指令"+ currentIns + "已在内存中" 
            }
        }
    }
    
    async function algo_LRU(){
        mpage_num_of_LRU = 0 // 缺页数要置零
        var cur_ins = -1  //当前指令
        var missing_flag = 0 //发生缺页置1
        var pos
        var direction = 2
        /*direction作为指令执行方向有四种情况：
            2 :顺序执行下一条指令，且上一步是向后跳转
            -1:跳转到前地址部分0－m-1中的某个指令处
            -2 :顺序执行下一条指令，且上一步是向前跳转
            1 :跳转到后地址部分m1+2~319中的某条指令处*/
        cur_ins = Math.round( Math.random() * (INSTRUCTIONS-1) )
        while( count_instuctions < INSTRUCTIONS ){
            var currentIns = cur_ins  //记录本次循环执行的指令
            missing_flag = 0
            pos = -1
            await sleep(now_rate.innerHTML)//睡眠

            //改变跳转方向和下一条指令
            if(direction === 2){//顺序执行指令
                cur_ins ++  //顺序下一条
                direction = -1//往前跳
            }
            else if(direction === -1){
                //向前跳转
                if (cur_ins < 2) {
                    continue
                }
                //Math.floor会向下取整
                cur_ins = Math.floor(Math.random() * (cur_ins - 1))
                //切换至顺序执行
                direction = -2
            }
            else if(direction === -2){//顺序执行指令，且上一步是向后跳转
                cur_ins ++
                direction = 1//往前条
            }
            else if(direction === 1){//往跳后
                if (cur_ins > INSTRUCTIONS - 2) {
                    continue
                }
                cur_ins = Math.floor( Math.random() * (INSTRUCTIONS - (cur_ins + 2)) + (cur_ins + 2) )
                direction = 2 //顺序执行且上一步往后跳
            }
            //越界处理，增加健壮性
            if (cur_ins < 0) {
                cur_ins = -1
                direction = 1//往后跳
                continue
            }
            else if (cur_ins >= INSTRUCTIONS) {
                cur_ins = INSTRUCTIONS + 1
                direction = -1//往前跳
                continue
            }

            if(in_memory(currentIns) === -1){
                //当前指令不在内存中，发生缺页
                missing_flag = 1
                if(mem_not_full() !== -1){
                    //还有空的内存块
                    pos = mem_not_full()
                    memory[mem_not_full()] = Math.floor(currentIns / PAGE_OF_INSTRUCTION)
                }
                else{
                    //内存块已经占满
                    pos = find_block()
                    memory[pos] = Math.floor(currentIns / PAGE_OF_INSTRUCTION)
                    mpage_LRU.textContent = mpage_num_of_LRU
                    mrate_LRU.textContent = mpage_num_of_LRU / INSTRUCTIONS
                }
                mpage_num_of_LRU++ 
            }
            count_instuctions++
            loading_bar.value = count_instuctions
            now_page.innerHTML = count_instuctions

            record[Math.floor(currentIns / PAGE_OF_INSTRUCTION)] = count_instuctions
            
            //结果输出
            var row = document.getElementById("memory_table").insertRow()
            row.insertCell(0).innerHTML = count_instuctions
            row.insertCell(1).innerHTML ="第" + currentIns + "条"
            for (let i = 0;i < 4;i++){
                row.insertCell(i+2).innerHTML = 
                memory[i] == undefined ? "--" : memory[i]
            }
            //内存块的值导到html中
            block1.innerHTML = memory[0]
            block2.innerHTML = memory[1]
            block3.innerHTML = memory[2]
            block4.innerHTML = memory[3]
            if(missing_flag === 1){
                row.insertCell(6).innerHTML = "缺页 "
                row.insertCell(7).innerHTML = "调入第" + (pos + 1) + "个内存块"
            }
            else{
                row.insertCell(6).innerHTML = "不缺页"
                row.insertCell(7).innerHTML = "指令"+ currentIns + "已在内存中" 
            }
        }
    }
    // 点击按钮模拟过程函数
    function start() {
        start_btn.disabled = true//对开始按钮的保护
        init()//初始化
        $("#memory_table  tr:not(:first)").hide()
        algorithm()// 选择算法并执行
        start_btn.disabled = false //重新启用开始按钮
    }
    //监听点击按钮事件
    $("#start_btn").click(start)
})
