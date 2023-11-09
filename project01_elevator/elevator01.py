import sys
import time
from functools import partial

from PyQt5 import QtGui, QtWidgets
from PyQt5.QtGui import *
from PyQt5.QtWidgets import *
from PyQt5.QtCore import *
from PyQt5 import QtCore
from PyQt5.QtWidgets import QWidget


ELE_NUM_SELECTED = 0

#内部按键
btn_paint = 'QPushButton{background-color: #B0C4DE; \
    border-radius: 20px; \
    font-size: 50px; font-weight:bold; border: 2px solid #E0FFFF;} \
    QPushButton:hover{background-color: #778899; \
    border-radius: 20px; \
    font-size: 50px; font-weight:bold;text-decoration:underline border: 2px solid #E0FFFF;} \
    '

# 等待态电梯
waiting_btn_paint = 'QPushButton{background-color: #CCCCFF; \
    border-radius: 20px; \
    font-size: 50px; font-weight:bold; border: 2px solid #E0FFFF;} \
    QPushButton:hover{background-color: #e54d42; \
    border-radius: 20px; border: 2px solid #E0FFFF; \
    font-size: 50px; font-weight:bold;text-decoration:underline} \
    QPushButton:pressed{background-color: #6739b6; \
    border-radius: 20px; border: 2px solid #E0FFFF; \
    font-size: 50px; font-weight:bold;text-decoration:underline}'
# 报警键
warning_paint = 'QPushButton{background-color: #CCCCFF; \
    border-radius: 20px; border: 2px solid #E0FFFF; \
    font-size: 30px; font-weight:bold;} \
    QPushButton:hover{background-color: #B0E0E6; \
    border-radius: 20px; border: 2px solid #E0FFFF; \
    font-size: 30px; font-weight:bold;text-decoration:underline} \
    QPushButton:pressed{background-color: #87CEFA; \
    border-radius: 20px; border: 2px solid #E0FFFF; \
    font-size: 30px; font-weight:bold;text-decoration:underline}'
# 开关门键
close_open_paint = 'QPushButton{background-color: #CCCCFF; \
    border-radius: 20px; border: 2px solid #E0FFFF; \
    font-size: 30px; font-weight:bold;} \
    QPushButton:hover{background-color: #B0E0E6; \
    border-radius: 20px; border: 2px solid #E0FFFF; \
    font-size: 30px; font-weight:bold;text-decoration:underline} \
    QPushButton:pressed{background-color: #87CEFA; \
    border-radius: 20px; border: 2px solid #E0FFFF; \
    font-size: 30px; font-weight:bold;text-decoration:underline}'
# 电子显示管
lcd_paint = 'QLCDNumber{background-color:#CCCCFF; \
    color: DodgerBlue;\
    border-radius: 20px; border: 2px solid DodgerBlue; \
    font-size: 600px; font-weight:bold;font-size: 30px;font-weight:bold;} \
    QLCDNumber::number {border-right: 3px solid #999;font-size: 50px; \
    }'



# 外部按键
outer_btn_paint = 'QPushButton{background-color: #DDA0DD; \
    border-radius: 7px; border: 2px solid #E0FFFF; \
    font-size: 35px; font-weight:bold;} \
    QPushButton:hover{background-color: #87CEFA; \
    border-radius: 7px; border: 2px solid #E0FFFF; \
    font-size: 35px; font-weight:bold;text-decoration:underline} \
    QPushButton:pressed{background-color: #1cbbb4; \
    border-radius: 7px; border: 2px solid #E0FFFF; \
    font-size: 35px; font-weight:bold;text-decoration:underline}'
outer_btn_paint_2 = 'QPushButton{background-color: #B0C4DE; \
    border-radius: 7px; border: 2px solid #E0FFFF; \
    font-size: 35px; font-weight:bold;} \
    QPushButton:hover{background-color: #87CEFA; \
    border-radius: 7px; border: 2px solid #E0FFFF; \
    font-size: 35px; font-weight:bold;text-decoration:underline} \
    QPushButton:pressed{background-color: #1cbbb4; \
    border-radius: 7px; border: 2px solid #E0FFFF; \
    font-size: 35px; font-weight:bold;text-decoration:underline}'
# 按钮

# 说明文本框


class sjm_UI(QWidget):
    global ELE_NUM_SELECTED
    
    def __init__(self):
        super(sjm_UI, self).__init__()

        grid = QGridLayout()
        # 电子显示管名称
        for i in range(5):
            word = QLabel('电梯' + str(i+1))
            word.setObjectName('name%d' % i)
            word.setStyleSheet("QLabel{border-radius:10px;border: 1px white; \
                                background-color:#e0e0e0;color:#0000AA; height:60; font-size:40px; \
                                font-family: SimHei;font-weight: 700;}")
            word.setAlignment(Qt.AlignCenter)
            grid.addWidget(word, 0, i)

        # 电子显示管 名称lcd0-4
        for i in range(5):
            lcd = QLCDNumber()
            lcd.setStyleSheet(lcd_paint)
            # lcd.setFont(QFont("Consolas", 24, QFont.Bold))
            lcd.setObjectName('lcd%d' % i)
            # lcd.setSegmentStyle(QLCDNumber.Filled)
            grid.addWidget(lcd, 1, i, 2, 1)


        # 电梯选择按钮 名称rbtn0-4
        for i in range(5):
            rbtn = QRadioButton('电梯%d' % (i+1))
            rbtn.setStyleSheet('font-weight : bold;')
            rbtn.setFont(QFont('heiti', 12))
            rbtn.setObjectName('rbtn%d' % i)
            rbtn.clicked.connect(partial(radiobtn_clicked, i))
            grid.addWidget(rbtn, 5, i)
            if i == 0:
                rbtn.setChecked(True)


        # 每个电梯的按钮 下方电梯名字btn0-19
        for i in range(20):
            btn = QPushButton('%d层' % (i+1))
            btn.setObjectName('btn%d' % i)
            btn.clicked.connect(partial(inner_floor_clicked, i))
            btn.setMinimumSize(200, 100)
            grid.addWidget(btn, (i // 5) + 7, i % 5)
        
        #报警键、开关键
        btn0 = QPushButton('报警')
        btn0.setObjectName('btn_warning')
        btn0.clicked.connect(warning_clicked)
        btn0.setMinimumSize(150, 70)
        btn0.setStyleSheet(warning_paint)
        grid.addWidget(btn0, 7, 6, 1, 1)

        btn1 = QPushButton('开')
        btn1.setObjectName('btn_open')
        btn1.clicked.connect(open_clicked)
        btn1.setMinimumSize(150, 70)
        btn1.setStyleSheet(close_open_paint)
        grid.addWidget(btn1, 8, 6, 1, 1)

        btn2 = QPushButton('关')
        btn2.setObjectName('btn_close')
        btn2.clicked.connect(close_clicked)
        btn2.setMinimumSize(150, 70)
        btn2.setStyleSheet(close_open_paint)
        grid.addWidget(btn2, 9, 6, 1, 1)


        # 下方说明文字
        info = QTextEdit()
        info.setMinimumSize(100, 300)
        info.setObjectName('info')
        info.setFontPointSize(10)   # 定义格式字体大小
        info.setStyleSheet('border: 1px solid grey; border-radius:3px; font: 75 10pt "微软雅黑";')
        info.setText("电梯交互信息如下：")
        grid.addWidget(info, 11, 0, 9, 5)
        
        #右侧的说明文字
        # scrollArea = QScrollArea()
        # scrollArea.setWidgetResizable(True)  # 这条不加无法显示里面的控件
        note = QTextBrowser()
        note.setText("<h4>欢迎来到2151299苏家铭的电梯调度小程序！</h4> \
                    <h6>您可以：</h6><h6>1、根据需求选择电梯</h6><h6>2、左侧按下电梯内部楼层</h6> \
                    <h6>3、右侧按下电梯外部楼层</h6><h6>电子显示管显示了电梯此时的楼层信息</h6>")
        note.setStyleSheet('border: 1px solid black; border-radius:3px; font: 75 10pt "微软雅黑";')
        note.setObjectName('note')
        grid.addWidget(note, 0, 7, 3, 2)


        # 右侧的全局电梯按钮 名称floor_up或down 0-19
        # grid -- scrollArea -w2- grid2
        grid2 = QGridLayout()
        scro = QScrollArea()
        w2 = QWidget()
        scro.setMinimumSize(350, 600)
        
        # scro.setStyleSheet('background-color: red')
        # w2.setStyleSheet('background-color: red')
        grid.addWidget(scro, 5, 7, 20, 1)

        for i in range(20):
            btn1 = QPushButton('%d层 ↑' % (20-i))
            btn2 = QPushButton('%d层 ↓' % (20-i))
            btn1.setObjectName('floor_up%d' % (19-i))
            btn2.setObjectName('floor_down%d' % (19-i))
            btn1.clicked.connect(partial(outer_floor_clicked, 'up', 19-i))
            btn2.clicked.connect(partial(outer_floor_clicked, 'down', 19-i))
            btn1.setStyleSheet(outer_btn_paint)
            btn2.setStyleSheet(outer_btn_paint)
            grid2.addWidget(btn1, i, 0)
            grid2.addWidget(btn2, i, 1)

        w2.setLayout(grid2)
        scro.setWidget(w2)


        self.setLayout(grid)
        # grid2 = QGridLayout()
        # grid.addLayout(grid2)
        self.setWindowTitle('苏家铭的电梯调度小程序')
        self.setStyleSheet('background-color: #e0e0e0')

        # window_pale = QtGui.QPalette()
        # window_pale.setBrush(self.backgroundRole(), QtGui.QBrush(QtGui.QPixmap("32.png").scaled(self.width(), self.height())))
        # self.setPalette(window_pale)
        self.show()


        # timer = QTimer()



class ele_thread(QThread):
       # 实例化一个信号对象
    trigger = pyqtSignal(int)

    def __init__(self, ELE_NUM):
        super(ele_thread, self).__init__()
        self.ele_num = ELE_NUM
        self.trigger.connect(check)

    def run(self):
        while (1):
            # print(stop_ele)
            if stop_ele[self.ele_num] == 1 or stop_ele[self.ele_num + 5] == 1:
                print("elevator "+str(self.ele_num + 1)+"stop in floor " + str(now_floor[self.ele_num] + 1))
                print("elevator "+str(self.ele_num + 1)+" open")
                obj = sjm.findChild(QTextEdit, 'info')
                obj.append("电梯"+str(self.ele_num + 1)+"停在了第" + str(now_floor[self.ele_num] + 1) + "层！")
                stop_ele[self.ele_num] = 0
                # sjm.findChild(QLabel, 'stop_btn%d' % self.ele_num).setStyleSheet("background-image: url(32.png)")
                # sjm.findChild(QPushButton, 'stop_btn%d' % self.ele_num).setStyleSheet("QPushButton{background-image: url(./32.png)}")
                stop_ele[self.ele_num + 5] = 0
            self.trigger.emit(self.ele_num)
            time.sleep(1)
            # 停留标识褪色
            # if stop_ele[self.ele_num] == 0 or stop_ele[self.ele_num + 5] == 0:
            #     obj = sjm.findChild(QLabel, 'stop_btn%d' % self.ele_num)
            #     obj.setPixmap(QPixmap(''))
            # sjm.findChild(QLabel, 'stop_btn%d' % self.ele_num).setStyleSheet('')


# 检查电梯的上下行状态并更改 按钮的状态并更改 绘制电梯按钮
def check(ele_num):
    global ELE_NUM_SELECTED  # 声明全局变量才能修改
    if warning_ele[ele_num] == 0:  # WARNING
        # print("up: " + str(outer_goal_floor[0]))
        # print("down: " + str(outer_goal_floor[1]))
        # 通过电梯状态改变电梯楼层
        if state_ele[ele_num] == 1:  # 向上状态
            now_floor[ele_num] += 1
        elif state_ele[ele_num] == -1:  # 向下状态
            now_floor[ele_num] -= 1
        else:  # 悬停状态
            pass

        # 改变完楼层后当然也要改变LCD的显示和该层内部按钮的颜色

        sjm.findChild(QLCDNumber, "lcd%d" % ele_num).display(now_floor[ele_num] + 1)

        # if (len(list(total_goal_floor[ele_num])) != 0):  # 消除只按外部会偏移一层的bug
            # if (now_floor[ele_num] <= max(list(total_goal_floor[ele_num]))) or (now_floor[ele_num] >= min(list(total_goal_floor[ele_num]))):

        # 因为五个电梯共用一个该层内部按钮 所以应该按ELE_NUM_SELECTED分情况
        paint_inner_floor_block(ELE_NUM_SELECTED)

        #  改变外部电梯颜色 + 停顿标识 + 移除目标
        if state_ele[ele_num] == 1:  # 向上
            if (now_floor[ele_num] in inner_goal_floor[ele_num]) or (now_floor[ele_num] in outer_goal_floor[0]):
                stop_ele[ele_num] = 1  # 要停
            if (now_floor[ele_num] not in outer_goal_floor[1]):
                total_goal_floor[ele_num].discard(now_floor[ele_num])  # 移除总目标
            inner_goal_floor[ele_num].discard(now_floor[ele_num])  # 移除内部目标
            outer_goal_floor[0].discard(now_floor[ele_num])  # 移除外部目标
            sjm.findChild(QPushButton, "floor_up%d" % (now_floor[ele_num])).setStyleSheet(outer_btn_paint)  # 外部按钮褪色
        elif state_ele[ele_num] == -1:  # 向下
            if (now_floor[ele_num] in inner_goal_floor[ele_num]) or (now_floor[ele_num] in outer_goal_floor[1]):
                stop_ele[ele_num + 5] = 1  # 要停
            if (now_floor[ele_num] not in outer_goal_floor[0]):
                total_goal_floor[ele_num].discard(now_floor[ele_num])  # 移除总目标
            inner_goal_floor[ele_num].discard(now_floor[ele_num])  # 移除内部目标
            outer_goal_floor[1].discard(now_floor[ele_num])  # 移除外部目标
            sjm.findChild(QPushButton, "floor_down%d" % (now_floor[ele_num])).setStyleSheet(outer_btn_paint)  # 外部按钮褪色
        elif state_ele[ele_num] == 0:  # 悬停
            if (now_floor[ele_num] in inner_goal_floor[ele_num]) or (now_floor[ele_num] in outer_goal_floor[1]):
                stop_ele[ele_num + 5] = 1  # 要停
            elif (now_floor[ele_num] in outer_goal_floor[0]):
                stop_ele[ele_num] = 1  # 要停

            # if (now_floor[ele_num] not in outer_goal_floor[0]):
            total_goal_floor[ele_num].discard(now_floor[ele_num])  # 移除总目标
                
            inner_goal_floor[ele_num].discard(now_floor[ele_num])  # 移除内部目标
            outer_goal_floor[0].discard(now_floor[ele_num])  # 移除外部目标
            outer_goal_floor[1].discard(now_floor[ele_num])  # 移除外部目标
            sjm.findChild(QPushButton, "floor_up%d" % (now_floor[ele_num])).setStyleSheet(outer_btn_paint)  # 外部按钮褪色
            sjm.findChild(QPushButton, "floor_down%d" % (now_floor[ele_num])).setStyleSheet(outer_btn_paint)  # 外部按钮褪色
        
        # 改变状态
        if state_ele[ele_num] == 1:  # 向上状态
            if len(list(total_goal_floor[ele_num])) == 0 :  # 总目标已经空了 那么状态设为悬停
                state_ele[ele_num] = 0
            elif max(list(total_goal_floor[ele_num])) < now_floor[ele_num]:  # 总目标不空 且 此时电梯在最大目标的上方
                state_ele[ele_num] = -1
            elif (max(list(total_goal_floor[ele_num])) == now_floor[ele_num]):  # 总不空 且 最大外部向下等于此时电梯
                state_ele[ele_num] = 0
        elif state_ele[ele_num] == -1:  # 向下状态
            if len(list(total_goal_floor[ele_num])) == 0:  # 总已经空了 就设为悬停
                state_ele[ele_num] = 0
            elif min(list(total_goal_floor[ele_num])) > now_floor[ele_num]:  # 总不空 且 此时电梯在最小目标的下方
                state_ele[ele_num] = 1
            elif (min(list(total_goal_floor[ele_num])) == now_floor[ele_num]):  # 总不空 且 此时最小外部向上等于此时电梯
                state_ele[ele_num] = 0
        else:  # 悬停状态
            if len(list(total_goal_floor[ele_num])) == 0:  # 总已经空了 仍为悬停
                pass
            elif min(list(total_goal_floor[ele_num])) < now_floor[ele_num]:  # 总不空 且 此时电梯在最小目标的上方
                state_ele[ele_num] = -1
            elif max(list(total_goal_floor[ele_num])) > now_floor[ele_num]:  # 总不空 且 此时电梯在最大目标的下方
                state_ele[ele_num] = 1


# 根据不同电梯的情况 绘制内部楼层按钮
def paint_inner_floor_block(ele_num):
    # print("inner: " + str(inner_goal_floor[ele_num]))
    for i in range(20):
        if (i in inner_goal_floor[ele_num]):
            sjm.findChild(QPushButton, 'btn%d' % i).setStyleSheet(btn_paint)
            
        else:
            sjm.findChild(QPushButton, 'btn%d' % i).setStyleSheet(waiting_btn_paint)
    # 绘制报警键
    if warning_ele[ele_num] == 1:
        sjm.findChild(QLCDNumber, "lcd%d" % ele_num).display('--')
    
# 事件：某部电梯内部楼层按键被点击 响应：1该电梯的目标楼层增加 2打印说明文字
def inner_floor_clicked(i):  # 参数为电梯号和所按楼层
    global ELE_NUM_SELECTED
    ele_num = ELE_NUM_SELECTED 
    inner_goal_floor[ele_num].add(i)
    total_goal_floor[ele_num].add(i)
    obj = sjm.findChild(QTextEdit, 'info')
    obj.append("电梯"+ str(ele_num + 1) +"的内部楼层" + str(i+1) + "被按下！")
    # print("elevator%d goals are" % ele_num)


# 事件：外部电梯楼层按键被点击 响应：1目标楼层增加 2打印说明文字 3外部按钮变色
def outer_floor_clicked(up_or_down, i):  # 参数为上下和所按楼层
    global ELE_NUM_SELECTED
    # print('press floor_{0}{1}'.format(up_or_down, i))
    # 添加外部目标数组
    if up_or_down == "up":
        outer_goal_floor[0].add(i)
    elif up_or_down == "down":
        outer_goal_floor[1].add(i)

    total_goal_floor[ELE_NUM_SELECTED].add(i)
    obj = sjm.findChild(QTextEdit, 'info')
    obj.append("外部楼层" + str(i+1) + up_or_down + "被按下！")

    sjm.findChild(QPushButton, "floor_{0}{1}".format(up_or_down, i)).setStyleSheet(outer_btn_paint_2)  # 外部按钮变色

    # print(outer_goal_floor[0])
    # print(outer_goal_floor[1])


# 事件：电梯选项按钮被点击 响应：修改全局ELE_NUM_SELECTED值
def radiobtn_clicked(ele_num):  # 参数为所选择的电梯号
    global ELE_NUM_SELECTED  # 声明全局变量才能修改
    ELE_NUM_SELECTED = ele_num
    # print("ELEVATOR%d" % (ELE_NUM_SELECTED))


# 事件：按下报警键 响应：1该电梯暂停状态被切换 2打印说明文字
def warning_clicked():
    global ELE_NUM_SELECTED
    ele_num = ELE_NUM_SELECTED
    obj = sjm.findChild(QTextEdit, 'info')
    if warning_ele[ele_num] == 0:  # 运行态
        warning_ele[ele_num] = 1  # 切换至暂停态
        obj.append("电梯" + str(ELE_NUM_SELECTED + 1) + "的报警键被按下，暂停使用！")
        print("elevator" + str(ele_num + 1) + "stop")
    elif warning_ele[ele_num] == 1:  # 暂停态
        warning_ele[ele_num] = 0  # 切换至运行态
        print("elevator" + str(ele_num + 1) + "run")
        obj.append("电梯" + str(ELE_NUM_SELECTED + 1) + "的报警键再次被按下，继续使用！")
    print(warning_ele[ele_num])
    # 自己总目标=0
    total_goal_floor[ele_num] = set()
    # 外部目标再分配  
    for i in range(5):
        if warning_ele[i] == 0:
            for j in outer_goal_floor[0]:
                total_goal_floor[i].add(j)
            for j in outer_goal_floor[1]:
                total_goal_floor[i].add(j)
            break

# 事件：按下开门键 响应：1打印说明文字
def open_clicked():
    global ELE_NUM_SELECTED
    obj = sjm.findChild(QTextEdit, 'info')
    if state_ele[ELE_NUM_SELECTED] == 0:
        obj.append("电梯" + str(ELE_NUM_SELECTED + 1) + "开门！")
    elif state_ele[ELE_NUM_SELECTED] == 1:
        obj.append("电梯" + str(ELE_NUM_SELECTED + 1) + "开门键被按下，但还在上升！")
    elif state_ele[ELE_NUM_SELECTED] == -1:
        obj.append("电梯" + str(ELE_NUM_SELECTED + 1) + "开门键被按下，但还在下降！")


# 事件：按下关门键 响应：1打印说明文字
def close_clicked():
    global ELE_NUM_SELECTED
    obj = sjm.findChild(QTextEdit, 'info')
    if state_ele[ELE_NUM_SELECTED] == 0:
        obj.append("电梯" + str(ELE_NUM_SELECTED + 1) + "关门！")
    elif state_ele[ELE_NUM_SELECTED] == 1:
        obj.append("电梯" + str(ELE_NUM_SELECTED + 1) + "关门键被按下，但还在上升！")
    elif state_ele[ELE_NUM_SELECTED] == -1:
        obj.append("电梯" + str(ELE_NUM_SELECTED + 1) + "关门键被按下，但还在下降！")


if __name__ == '__main__':
    app = QApplication(sys.argv)
    sjm = sjm_UI()
    sjm.resize(400, 200)
    # sjm.show()

    now_floor = []  # 记录五个电梯此时的层数 0-19
    for i in range(5):
        now_floor.append(0)
    
    ele1 = set([])  # 使用集合而不是列表可以避免重复按同一层楼的情况
    ele2 = set([])
    ele3 = set([])
    ele4 = set([])
    ele5 = set([])
    inner_goal_floor = [ele1, ele2, ele3, ele4, ele5]  # 记录五个电梯内部的目标层数 0-19
    
    ele1 = set([])
    ele2 = set([])
    ele3 = set([])
    ele4 = set([])
    ele5 = set([])
    total_goal_floor = [ele1, ele2, ele3, ele4, ele5]  # 记录五个电梯内部的目标层数 0-19

    up = set([])
    down = set([])
    outer_goal_floor = [up, down]  # 记录外部电梯的目标层数 0-19 outer_goal_floor[0]：上升 outer_goal_floor[1]：下降
 
        
    state_ele = []  # 记录电梯的状态 0表示悬停 1表示上升 -1表示下降
    for i in range(5):
        state_ele.append(0)

    stop_ele = []  # 记录五部电梯是否要停下 0:不停 1：要停 
    for i in range(10):
        stop_ele.append(0)

    warning_ele = []  # 记录每部电梯是否报警 0:正常运行 1:暂停
    for i in range(5):
        warning_ele.append(0)

    # 创建五个线程
    ele_t1 = ele_thread(0)
    ele_t2 = ele_thread(1)
    ele_t3 = ele_thread(2)
    ele_t4 = ele_thread(3)
    ele_t5 = ele_thread(4)

    # ele_t6 = painttread()
    # 开始线程
    ele_t1.start()
    ele_t2.start()
    ele_t3.start()
    ele_t4.start()
    ele_t5.start()

    sys.exit(app.exec_())