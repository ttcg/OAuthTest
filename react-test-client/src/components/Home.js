import React, { useEffect, useState } from 'react';
import Oidc from 'oidc-client'

export default function Home() {

    const [currentUser, setCurrentUser] = useState(null)
    const [result, setResult] = useState(null)

    var config = {
        authority: "https://localhost:44378",
        client_id: "react-test-client",
        redirect_uri: "http://localhost:3000/callback",
        response_type: "code",
        scope: "openid profile oauthtestapistudents",
        post_logout_redirect_uri: "http://localhost:3000/index.html",
    };

    var mgr = new Oidc.UserManager(config);

    useEffect(() => {
        mgr.getUser().then(function (user) {
            if (user) {
                console.log("User logged in", user.profile);
                setCurrentUser(user)
                setResult(user.profile)
            }
            else {
                console.log("User not logged in");
            }
        });
    }, []);

    const setLoading = () => {
        setResult('Loading......')
    }

    const login = () => {
        setLoading()
        mgr.signinRedirect();
    }

    const logout = () => mgr.signoutRedirect();

    const callApi = () => {
        setLoading()
        if (currentUser) {
            var url = "https://localhost:44367/api/students/f1e5f27b-8fbc-4baf-b769-f82c25ed551e";

            var xhr = new XMLHttpRequest();
            xhr.open("GET", url);
            xhr.onload = function () {
                console.log(xhr.status, JSON.parse(xhr.responseText));
                setResult(JSON.parse(xhr.responseText))
            }
            xhr.setRequestHeader("Authorization", "Bearer " + currentUser.access_token);
            xhr.send();
        }
        else {
            setResult("User not logged in")
        }
    }

    return (
        <>
            {!currentUser && <button className="button" onClick={() => login()}>Login</button>}
            <button className="button" onClick={() => callApi()}>Call Student Api to get Student Detail</button>

            {currentUser && <button className="button" onClick={() => logout()}>Logout</button>}
            {result &&
                <div className="result">
                    <pre>
                        {JSON.stringify(result, null, 2)}
                    </pre>
                </div>
            }
        </>
    );
}