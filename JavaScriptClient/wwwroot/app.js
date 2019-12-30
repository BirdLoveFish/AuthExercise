var config = {
    authority: "http://localhost:5000",
    client_id: "client_id_js",
    redirect_uri: "http://localhost:5004/callback.html",
    response_type: "id_token token",
    scope: "openid profile ApiOne",
};
var userManager = new Oidc.UserManager(config);


function login() {
    userManager.signinRedirect();
}



