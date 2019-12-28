### 这个自定义的授权服务器必须运行在浏览器上，或者说可以运行在iis express，或者发布到iis后,而且使用搜狗浏览器是不行的，必须使用google浏览器。

### 而直接使用命令行来启动这个程序是不行的

### 由于会有多个项目会结合在一起使用，所以声明一下目前的项目进度，在OAuth项目文件夹下有Client和Server2个项目

### 自定义OAuth项目总结
Server是授权认证的服务器，作用是验证token的合法性以及颁发token

Client是客户端，当用户不携带token时，会向服务器的授权端点发起请求，然后输入用户信息，服务器确认信息之后再颁发token，然后代表验证通过了。
此时客户端中User已经被赋值了，然后Client会给这个用户一个Cookie，以后就可以pingjie这个Cookie来请求这个地址了。而不需要再次向Server验证了。

Api就是客户端需要访问的接口，这个接口不需要自己定义规则，只要定义一个Requirement
让Server帮我们去验证token的正确性即可，返回200即表示该token正确

Client->自身加密接口							解析Token，User赋值->进入自身接口->访问加密Api接口
	/\		|										/\									|
	|		\/										|									|
	|		Server.Authorize->Username&Password->Server.Token							|
	|					验证正确<-Server.Validate										|
	|					|					/\											|
	|					\/					|											\/
访问api接口<-Requirement通过		带着token向服务器验证token<-获取上下文中的token<-定义Requirement