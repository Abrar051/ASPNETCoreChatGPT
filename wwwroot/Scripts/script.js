//const IntroOption;


function IntroductoryOption() {
    var settings = {
        "url": "../home/IntroductoryOption",
        "method": "GET",
        "timeout": 0,
    };


    $.ajax(settings).done(function (response) {
        debugger;
        console.log(response);
        //IntroOption = response;
        sessionStorage.setItem('IntroOption', response);
    });

   

}

function addChatMessageBegin(name, img, side) {
    debugger;
    var introOptionString = sessionStorage.getItem('IntroOption');
    var introOptionList = introOptionString.split(',');
    console.log(introOptionList);
    //var IntroOptions = sessionStorage.getItem('IntroOption');
    let msgHTML = `
                    <div class="msg ${side}-msg">
                        <div class="msg-img" style="background-image: url(${img})"></div>

                        <div class="msg-bubble">
                        <div class="msg-info">
                            <div class="msg-info-name">${name}</div>
                            <div class="msg-info-time">${formatDate(new Date())}</div>
                        </div>

                        <div class="msg-text">`;

    if (Array.isArray(introOptionList)) {
        IntroOptions.forEach(option => {
            msgHTML += `<div><button class="btn btn-light" onclick="handleOptionClick('${option}')">${option}</button></div><p></p>`;
        });
    } else {
        msgHTML += `${IntroOptions}`;
    }

    msgHTML += `</div>
                    </div>
                    </div>`;

    $(".msger-chat").append($(msgHTML));

    if (side === "left") {
        var loaderHTML = `<div id="dvLoader"><img class="imgLoader" src="/images/loader.gif" /></div>`;
        $(".msger-chat").append($(loaderHTML));
    }

    $(".msger-chat").scrollTop($(".msger-chat").prop("scrollHeight"));

    return false;
}