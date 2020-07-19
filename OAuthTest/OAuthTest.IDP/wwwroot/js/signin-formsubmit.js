document.getElementById("frmLogin").addEventListener("submit", disableButton);

function disableButton() {
    document.getElementById('loginButton').disabled = 'disabled';
    return true;
}