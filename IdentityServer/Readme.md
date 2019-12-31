# 添加claim

1.用户新增一个cliam
2.将这个cliam放到一个scope中
3.只要正确的请求这个scope就能获得这个cliam


# 添加api scope
1.存在一个scope(ApiResource IdentityResource)
2.Configuration允许访问这个scope
3.Client要请求这个scope


# 关于access_token和id_token
id_token表示用户信息，access_token表示访问其他接口的能力。
两者有一些共同的信息，又有一些不同的信息。

1.给用户增加一个claim，access_token会增加一个scope，而id_token会增加这个scope中的值。
前提是AlwaysIncludeUserClaimsInIdToken = true;

2.Api资源是一个scope，Identity资源也是一个scope。

3一个Cliam可以属于Api这个Scope，也可以属于Identity这个Scope，或者2者都属于。

4.当把一个Cliam添加到Api Scope中时，改变的是access_token，Api中的User会获得了这个claim

5.当把一个Cliam添加到Identity Scope中时，改变的是id_token，Client中的User会获得这个claim

# 将Configure配置移到sqlserver数据库

## 安装包

1. Nuget IdentityServer4.EntityFramework
2. Nuget Microsoft.EntityFrameworkCore.Tools
3. Nuget Microsoft.EntityFrameworkCore.SqlServer

## Startup配置

1. 在appsetting.json中添加连接字符串, Startup中获取连接字符串
2. 将IdentityServer的内存数据修改为sqlserver数据库
3. 将AppDbContext的内存数据修改为sqlserver数据库

## 迁移

0. PowerShell控制台的"默认项目"选择为IdentityServer
1. Add-Migration init[App] -c AppDbContext   -o Migrations/Application/ApplicationDb
2. Add-Migration init[Config] -c ConfigurationDbContext   -o Migrations/IdentityServer/ConfigurationDb
3. Add-Migration init[Persist] -c PersistedGrantDbContext  -o Migrations/IdentityServer/PersistedGrantDb

## 初始化数据库

1. 初始化3个数据库
scope.ServiceProvider.GetRequiredService<<DbContext>>().Database.Migrate();
2. 其中的ConfigurationDbContext需要将Configuration中的配置全部添加到数据库中，注意要保存
3. 添加测试用户



# 生成证书

