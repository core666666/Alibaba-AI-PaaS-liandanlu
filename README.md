# Alibaba-AI-PaaS-liandanlu
阿里巴巴通义千问AI PaaS大模型、炼丹炉大模型、ChatMemo接入、模型训练、VUE页面部署、后端部署、对接阿里云通义千问、语音转文本、简易搭建通义千问大模型聊天功能（支持PC端、移动端）

### 功能清单
| 序号 | 功能 | 进度 |
| ------ | ------ | ------ |
| 1 | 接入阿里云AI PaaS平台 | 完成 |
| 2 | 训练基础知识库并对接模型 | 完成 |
| 3 | 编写基础的对话页面(VUE) | 完成 |
| 4 | 支持语音输入功能 | 完成 |
| 5 | 本地存储聊天记录功能 | 1% |
| 6 | 支持多个会话窗口 | 0% |
| 7 | 导出聊天记录功能 | 0% |
| 8 | 登录授权功能 | 0% |

### 代码位置alibaba
###### getAccessToken()方法需要从阿里云创建AI PaaS的应用，并且开通【炼丹炉专属模型接口】，其中【AppKey】【AppSecret】需要填写
#### 该AccessToken有效期2h，到期自动刷新

#### 1.创建AI PaaS应用
[AI PaaS链接](https://open-dev.dingtalk.com/?spm=dd_developers.homepage.0.0.4ef84a978dtaWB#/)
[![OyvlLx.th.png](https://ooo.0x0.ooo/2024/02/28/OyvYMX.png)](https://img.tg/image/OyvlLx)

#### 2.复制AppKey、AppSecret
[![OyvlLx.th.png](https://ooo.0x0.ooo/2024/02/28/OyvlLx.png)](https://img.tg/image/OyvlLx)

#### 2.开通炼丹炉大模型权限
[![OyvlLx.th.png](https://ooo.0x0.ooo/2024/02/28/Oyvsci.png)](https://img.tg/image/OyvlLx)

#### 3.上传知识库进行训练
[炼丹炉大模型开发链接](https://open.dingtalk.com/document/ai-dev/basic-concepts)
[![OyvlLx.th.png](https://ooo.0x0.ooo/2024/02/28/OyvcBC.png)](https://img.tg/image/OyvlLx)

#### 4.选择并创建模型
[![OyvlLx.th.png](https://ooo.0x0.ooo/2024/02/28/OyvGza.png)](https://img.tg/image/OyvlLx)

#### 5.聊天首页（阿里云AI PaaS目前不支持连续对话，由程序模拟的连续对话）
[![OyvlLx.th.png](https://ooo.0x0.ooo/2024/03/28/Og28Jt.png)](https://img.tg/image/OyvlLx)

#### 6.连续聊天，提问
[![OyvlLx.th.png](https://ooo.0x0.ooo/2024/02/28/OyvdZL.png)](https://img.tg/image/OyvlLx)

#### 7.选择其他模型
[![OyvlLx.th.png](https://ooo.0x0.ooo/2024/02/28/OyvQJS.png)](https://img.tg/image/OyvlLx)

#### 8.对接语音转文本-创建项目（新账户有3个月的免费试用期）
[智能语音交互链接](https://help.aliyun.com/document_detail/72138.html?spm=a2c4g.119258.0.0.35d91dd8RD7r8V)
[![OyvlLx.th.png](https://ooo.0x0.ooo/2024/03/05/OyGCHS.png)](https://img.tg/image/OyvlLx)

#### 9.对接语音转文本-获得AccessToken（Token有效期为24h）
[获取Token链接](https://help.aliyun.com/document_detail/450514.html?spm=a2c4g.113251.0.0.63b24d82mz7R6r)
[![OyvlLx.th.png](https://ooo.0x0.ooo/2024/03/05/OyGwXN.png)](https://img.tg/image/OyvlLx)

#### 10.对接语音转文本-使用
[![OyvlLx.th.png](https://ooo.0x0.ooo/2024/03/05/OyG37a.png)](https://img.tg/image/OyvlLx)

#### 11.ChatMemo接入
[ChatMemo链接](https://solu-ai.dingtalk.com/#/apps)
[![OyvlLx.th.png](https://ooo.0x0.ooo/2024/03/28/Og2Xlx.png)](https://img.tg/image/OyvlLx)

#### 其他小功能1 海创网职位获取
###### 代码位置 jishianquan-haichuangController
#### 其他小功能2 微信小程序【冀时安全应急管理宣传教育平台】自动刷题、自动阅读资讯、自动获取积分（已经打包好了软件，具体软件就不透露了，跟着接口自己写写就成了）
###### 代码位置 jishianquan-jishianquanController

