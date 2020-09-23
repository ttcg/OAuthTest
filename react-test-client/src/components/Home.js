import React, { useEffect, useState } from 'react';
import Oidc from 'oidc-client'

export default function Home() {

    const [currentUser, setCurrentUser] = useState(null)
    const [access_token, setAccessToken] = useState(null)
    const [result, setResult] = useState(null)

    var config = {
        authority: "https://localhost:44378",
        client_id: "react-test-client",
        redirect_uri: "http://localhost:3000/callback",
        response_type: "code",
        scope: "openid profile oauthtestapistudents",
        post_logout_redirect_uri: "http://localhost:3000/index.html",
        automaticSilentRenew: true,
        silent_redirect_uri: 'http://localhost:3000/silent.html'
    };

    var mgr = new Oidc.UserManager(config);

    useEffect(() => {
        mgr.getUser().then(function (user) {
            if (user) {
                console.log("User logged in", user.profile);
                setCurrentUser(user)
                setResult(user.profile)
                setAccessToken(user.access_token)
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
            // setAccessToken(JSON.parse(sessionStorage.getItem('oidc.user:https://localhost:44378:react-test-client')).access_token);

            mgr.getUser().then(function (user) {

                setAccessToken(user.access_token)

                console.log(access_token)

                var url = "https://localhost:44367/api/students/f1e5f27b-8fbc-4baf-b769-f82c25ed551e";
                fetch(url, {
                    headers: {
                        'Content-Type': 'application/json',
                        "Authorization": "Bearer " + access_token,
                    }
                })
                    .then(response => response.json())
                    .then(
                        (data) => {
                            setResult(data)
                        }
                    );
            });
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
                        {access_token}
                    </pre>
                    <pre>
                        {JSON.stringify(result, null, 2)}
                    </pre>
                </div>
            }
        </>
    );
}