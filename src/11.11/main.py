from matplotlib import markers
import numpy as np 
import pandas as pd
import matplotlib.pyplot as plt
from sklearn.linear_model import LinearRegression, LogisticRegression
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import StandardScaler


plt.rcParams["font.sans-serif"] ='SimHei'
plt.rcParams['axes.unicode_minus'] = False

def linear_equation_1(year, sales):
    X = (year - 2008).reshape(-1, 1)
    X = np.concatenate([X], axis= -1)
    Y = sales
    lr = LinearRegression()
    lr.fit(X, Y)
    print('方程系数：', lr.coef_)
    print('方程截距：',lr.intercept_)
    # plt.plot(year, sales, 'o')
    f = lambda x: lr.coef_[0] * x + lr.intercept_
    plt.plot(year, f(year - 2008), c='red', linestyle='--', label='一次方程', marker='s')
    # plt.plot(np.array([12, 13]), np.array([4982, f(13)]), c='black')
    print('2021年交易额预测：', f(13))
    X_train, X_test, Y_train, Y_test = train_test_split(X, Y, test_size=0.3, random_state=0)
    print('R^2：', lr.score(X_test, Y_test))

def linear_equation_2(year, sales):
    X = (year - 2008).reshape(-1, 1)
    X = np.concatenate([X**2, X], axis= -1)
    Y = sales
    lr = LinearRegression()
    lr.fit(X, Y)
    print('方程系数：', lr.coef_)
    print('方程截距：',lr.intercept_)
    plt.plot(year, sales, 'o')
    f = lambda x: lr.coef_[0] * x**2 + lr.coef_[1] * x + lr.intercept_
    plt.plot(year, f(year - 2008), c='blue', linestyle='--', label='二次方程', marker='^')
    print('2021年交易额预测：', f(13))
    X_train, X_test, Y_train, Y_test = train_test_split(X, Y, test_size=0.3, random_state=0)
    print('R^2：', lr.score(X_test, Y_test))


def linear_equation_3(year, sales):
    X = (year - 2008).reshape(-1, 1)
    X = np.concatenate([X**3, X**2, X], axis= -1)
    Y = sales
    lr = LinearRegression()
    lr.fit(X, Y)
    print('方程系数：', lr.coef_)
    print('方程截距：',lr.intercept_)
    plt.plot(year, sales, 'o')
    f = lambda x: lr.coef_[0] * x**3 + lr.coef_[1] * x**2 + lr.coef_[2] * x + lr.intercept_
    plt.plot(year, f(year - 2008), c='green', linestyle='dashed', label='三次方程', marker='v')
    print('2021年交易额预测：', f(13))
    X_train, X_test, Y_train, Y_test = train_test_split(X, Y, test_size=0.3, random_state=0)
    print('R^2：', lr.score(X_test, Y_test))


df = pd.read_csv('./历年双十一交易额.csv', index_col = 0)
year = np.array(df.index.tolist())
sales = np.array(df.Sales.tolist())
plt.scatter(year, sales, marker='o')
plt.plot(year, sales, marker='o', label='真实值', c='black')
plt.xlabel('年份')
plt.ylabel('交易额(亿)')
plt.title('历年双十一交易额变化趋势预测')

linear_equation_1(year, sales)
linear_equation_2(year, sales)
linear_equation_3(year, sales)

plt.legend(loc='upper right')
plt.show()