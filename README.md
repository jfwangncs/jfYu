## 一个集合了各种.Net跨平台的工具

集合平时项目中使用的工具插件，主要支持.Net Core,兼容.Net 461,会持续更新。

## 有哪些？

- [缓存](https://github.com/jfwangncs/jfYu/tree/master/src/jfYu.Core/jfYu.Core.Cache) - 缓存组件支持Redis、本地缓存.
- [验证码](https://github.com/jfwangncs/jfYu/tree/master/src/jfYu.Core/jfYu.Core.Captcha) - 简单的验证码组件.
- [常用公共方法](https://github.com/jfwangncs/jfYu/tree/master/src/jfYu.Core/jfYu.Core.Common) - 常用的公共方法整合了很多。例如:配置文件读取、数据加解密、简单文件日志、NLog日志、AutoMapper注册等等.
- [Web平台组件](https://github.com/jfwangncs/jfYu/tree/master/src/jfYu.Core/jfYu.Core.CPlatform) - 针对web平台的组件主要作用为依赖注入和后期的微服务发展、然后会集成IdentityServer。
- [轻量级读写分离](https://github.com/jfwangncs/jfYu/tree/master/src/jfYu.Core/jfYu.Core.Data) - 简单的读写分离，包含统一的公共数据操作方法，便捷的分页功能.
- [网络请求](https://github.com/jfwangncs/jfYu/tree/master/src/jfYu.Core/jfYu.Core.jfYuRequest) - 封装了HttpRequest和HttpClient，提供了便捷的方式获取网页内容，下载文件.
- [MongoDB](https://github.com/jfwangncs/jfYu/tree/master/src/jfYu.Core/jfYu.Core.MongoDB) - 便捷操作MongoDB数据库.
- [RabbitMQ](https://github.com/jfwangncs/jfYu/tree/master/src/jfYu.Core/jfYu.Core.RabbitMQ) - 封装RabbitMQ，支持所有协议，可简单的发送接受数据.
- [Redis](https://github.com/jfwangncs/jfYu/tree/master/src/jfYu.Core/jfYu.Core.Redis) - 便捷操作Redis，支持Redis集群.
- [Email](https://github.com/jfwangncs/jfYu/tree/master/src/jfYu.Core/jfYu.Core.EMail) - 邮件发送功能.
- [Configuration](https://github.com/jfwangncs/jfYu/tree/master/src/jfYu.Core/jfYu.Core.Configuration) - 配置工具，web下自动读取配置文件，支持选项模式
- [Excel](https://github.com/jfwangncs/jfYu/tree/master/src/jfYu.Core/jfYu.Core.Excel) - 基于NPOI的组件，封装了很多便捷导入导出的方法，支持超大数据导出.
- [Word](https://github.com/jfwangncs/jfYu/tree/master/src/jfYu.Core/jfYu.Core.Word) - 基于NPOI的组件的word方法包括图片的模板插入.

## 未来

- 集成IdentityServer
- 加入小程序、公众号支持
