var x = 0;
var s = "";

console.log("hello!");

var theForm = document.getElementById("theForm");
theForm.hidden = true;

var button = document.getElementById("buyButton");
button.addEventListener("click", function () {
    alert("Buying item");
});
