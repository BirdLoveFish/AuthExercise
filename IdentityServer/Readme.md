# 添加claim步骤

1.用户新增一个cliam
2.定义一定IdentityResource的资源，新建一个scope，scope中包含了这个claim
3.IdentityServer中的Client需要添加这个Scope
4.MvcClient中的Scope也需要添加这个scope

# 添加api scope
1.存在一个Api项目，设置Audience为ApiOne
2.添加ApiResource，并注册到IdentityServer中
3.Configuration.Client中添加获取这个ApiOne的Scope
4.Client中需要请求这个ApiOne的Scope

# 关于access_token和id_token
id_token表示用户信息，access_token表示访问其他接口的能力。
两者有一些共同的信息，又有一些不同的信息。

1.当给Client添加Api的访问权限(scope中添加ApiOne)，则access_token中添加了scope:ApiOne,而id_token并未改变

2.options.ClaimActions.Clear();只会影响id_token

3.给用户增加一个claim，access_token会增加一个scope，而id_token会增加这个scope中的值。
前提是AlwaysIncludeUserClaimsInIdToken = true;

4.Api资源是一个scope，Identity资源也是一个scope。一个Cliam可以时域Api这个Scope，也可以属于
Identity这个Scope，或者2者都属于。当把一个Cliam添加到Api Scope中时，改变的是access_token。
access_token改变了，那么在Api中的User也就获得了这个claim。
当把一个Cliam添加到Identity Scope中时，改变的是id_token。id_token改变了，那么Client中的
User也就改变了