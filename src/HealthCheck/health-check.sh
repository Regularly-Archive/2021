#!/bin/bash

# 定义函数 check_http() ：
# 使用 curl 命令检查 HTTP 服务状态
# 要求返回如下 JSON 格式：{ "flag" : true }
# -s：设置静默连接，不显示连接时的连接速度、时间消耗等信息
check_http() {
    flag=$(curl -s $url | python -c "import sys, json; print(json.load(sys.stdin)['flag'])")
}

url=$1
check_http
date=$(date +%Y-%m-%d-%H:%M:%S) 
echo "当前时间为: $date"
echo "服务地址为: $url"
echo "是否健康: $flag"
if [ $flag == "True" ];then
   exit 0
else
   exit -1
fi