import React from 'react';
import Oidc from 'oidc-client'

export default function Callback() {

    new Oidc.UserManager({response_mode:"query"}).signinRedirectCallback().then(function() {
        window.location = "index.html";
    }).catch(function(e) {
        console.error(e);
    });

    return (
        <>
            Hello from Callback
        </>
    );
}