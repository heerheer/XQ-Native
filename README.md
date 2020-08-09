# XQ-Native (永久暂停更新，已删库)
让XQ先驱机器人兼容大部分CQ插件
> Jie2GG老大，永远滴神

### 推荐
我们推荐你们使用更频繁更新的c++编写的

**CQXQ** 插件获得更简单跟强大的功能实现

[CQXQ](https://github.com/w4123/CQXQ/)

## 更新日志
[点我](https://github.com/heerheer/XQ-Native/wiki/%E6%9B%B4%E6%96%B0%E6%97%A5%E5%BF%97)


## 使用教程
移至 [wiki](https://github.com/heerheer/XQ-Native/wiki)

# 出现问题？
请Githu本仓库提交Issue（见上方）

## 实现原理
![avatar](https://s1.ax1x.com/2020/08/07/aW3f8s.png)

### 注意
1. 使用本插件载入的CQ插件 **不会** 出现在先驱插件列表中
2. 本插件**暂时**不支持机器人QQ 大于 Int最大数的


## 目录结构

```
 └─先驱机器人主目录
     │  CQP.dll // XQ Native Bridge
     │
     ├─CQPlugins // 被XQ-Native加载的所有CQ插件
     │      ├─com.example.cqapp1 //某个目录
     │      │        │─ app.dll
     │      │        └─ app.jso
     │      │─com.example.cqapp2 //某个目录
     │      │        │─ app.dll
     │      │        └─ app.json
     │      └─com.example.cqapp3 //某个目录
     │               │─ app.dll
     │               └─ app.json
     └─Plugin // 插件文件夹
             XQNative.XQ.dll // XQ-Native桥接插件
             Native.XQ.Lib.XQ.dll // Native发送消息兼容插件
```

## 捐赠
> 此处捐赠二维码并不是本人，而是让我受益匪浅的Jie2GG老大
<img src="https://camo.githubusercontent.com/1d7bc1dd353cded28f993fd208e8347786c4be38/68747470733a2f2f6a69653267672e6769746875622e696f2f496d6167652f5765436861742e706e67" width="120">
<img src="https://camo.githubusercontent.com/9d1998e384f4f5a0e494271d639a4beb1c9823d9/68747470733a2f2f6a69653267672e6769746875622e696f2f496d6167652f416c69506c61792e706e67" width="120">
