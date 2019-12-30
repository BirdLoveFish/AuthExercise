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

