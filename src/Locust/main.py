import random
from locust import *
from locust import HttpUser, task


# 定义一个任务类，这个类名称自己随便定义，类继承TaskSequence 或 TaskSet类，所以要从locust中，引入TaskeSequence或TaskSet
# 当类里面的任务请求有先后顺序时，继承TaskSequence类， 没有先后顺序，可以使用继承TaskSet类
class MyTaskCase(HttpUser):
    # 初始化方法，相当于 setup
    def on_start(self):
        pass

    # @task python中的装饰器，告诉下面的方法是一个任务，任务就可以是一个接口请求，
    # 这个装饰器和下面的方法被复制多次，改动一下，就能写出多个接口
    # 装饰器后面带上(数字)代表在所有任务中，执行比例
    # 要用这个装饰器，需要头部引入 从locust中，引入 task
    @task(1)
    def getAllVehicleStatus(self):  # 一个方法， 方法名称可以自己改
        url = '/ResStatus.ResStatusSrv/GetAllVehicleStatus'  # 接口请求的URL地址
        self.headers = {"Content-Type": "application/json"}  # 定义请求头为类变量，这样其他任务也可以调用该变量
        data = { }  # post请求的 请求体
        # 使用self.client发起请求，请求的方法根据接口实际选,
        # catch_response 值为True 允许为失败 ， name 设置任务标签名称   -----可选参数
        rsp = self.client.post(url, json=data, headers=self.headers, catch_response=True, name='api_regist')
        if rsp.status_code == 200:
            rsp.success()
        else:
            rsp.failure('regist_ 接口失败！')

# 定义一个运行类 继承HttpLocust类， 所以要从locust中引入 HttpLocust类
class UserRun(TaskSet):
    task_set = MyTaskCase  # 定义固定的 task_set  指定前面的任务类名称
    wait_time = between(0.1, 3)  # 设置运行过程中间隔时间 需要从locust中 引入 between


'''
运行：
    在终端中输入：locust -f 被执行的locust文件.py --host=http://被测服务器域名或ip端口地址
    也可以不指定host
命令执行成功，会提示服务端口，如：*：8089
此时，则可通过浏览器访问机器ip:8089,看到任务测试页面
'''