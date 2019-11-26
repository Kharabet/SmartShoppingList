var SpeechRecognition = SpeechRecognition || webkitSpeechRecognition
var SpeechGrammarList = SpeechGrammarList || webkitSpeechGrammarList
var SpeechRecognitionEvent = SpeechRecognitionEvent || webkitSpeechRecognitionEvent

var products = JSON.parse($("#productsDb").val());
//var grammar = '#JSGF V1.0; grammar colors; public <color> = ' + products.join(' | ') + ' ;'

var grammar = `#JSGF V1.0;

grammar voicecommands;

 public <command> = <action> <object>;
<action> = /10/ add |/2/ remove |/1/ delete |/1/ throw;
<object> = ` + products.join(' | ') + ` ;`;

var recognition = new SpeechRecognition();
var speechRecognitionList = new SpeechGrammarList();
speechRecognitionList.addFromString(grammar, 1);
recognition.grammars = speechRecognitionList;
//recognition.continuous = false;
recognition.lang = 'ru-ru';
recognition.interimResults = false;
recognition.maxAlternatives = 1;

var diagnostic = document.querySelector('.output');
var bg = document.querySelector('html');
var hints = document.querySelector('.hints');
var $recordButton = $('#recButton');
var colorHTML = '';
hints.innerHTML = 'Натисніть на червоне коло та почніть додавати товари. Наприклад "додати сіль".';

$recordButton.on("click",
    function () {
        
        recognition.start();
            $recordButton.removeClass("notRec");
            $recordButton.addClass("Rec");
        console.log('Ready to receive a color command.');
    });

recognition.onresult = function (event) {
    // The SpeechRecognitionEvent results property returns a SpeechRecognitionResultList object
    // The SpeechRecognitionResultList object contains SpeechRecognitionResult objects.
    // It has a getter so it can be accessed like an array
    // The [last] returns the SpeechRecognitionResult at the last position.
    // Each SpeechRecognitionResult object contains SpeechRecognitionAlternative objects that contain individual results.
    // These also have getters so they can be accessed like arrays.
    // The [0] returns the SpeechRecognitionAlternative at position 0.
    // We then return the transcript property of the SpeechRecognitionAlternative object

    var last = event.results.length - 1;
    var color = event.results[last][0].transcript;
    var command = color.split(' ')[0].toLowerCase();
    var item = color.substr(color.indexOf(" ") + 1).toLowerCase();
    diagnostic.textContent = 'You wan to' + command + ' the' + item + '.';
    bg.style.backgroundColor = color;
    console.log('Confidence: ' + event.results[0][0].confidence);
    var action;
    if (command == "добавить" || command == "додати" || command == "add ") {
        action = "add-user-product";
    } else if (command == "видалити" || command == "прибрати" || command == "викинути" || command == "удалить" || command == "выкинуть" || command == "remove ") {
        action = "user-product-to-bin";
    } else {
        $.alert("Unknown action");
        return;
    }
    $.confirm({
        title: diagnostic.textContent,
        content: 'Simple confirm!',
        buttons: {
            Yes: function () {
                var userId = $("#UserProduct_UserId").val();

                $.get("/api/get-product-by-name",
                    { name: item }, function (resp) {
                        if (resp.success) {
                            var productId = resp.product.id;
                            var data = {
                                userId: userId,
                                quantity: 1,
                                productId: resp.product.id
                            };

                            $.post("/api/" + action, data).done(function(response) {
                                if (response.success === true) {
                                    $.alert("successfully addedd!");
                                    document.location.reload(true);
                                } else {
                                    $.alert("Something went wrong!");
                                }

                            });
                        } else {
                            $.alert("There is no such product! Try again!");
                        }
                    });
            },
            No: function () {
                $.alert('PLease repeate the name of product!');
                return;
            }
        }
    });

}

recognition.onspeechend = function () {
    recognition.stop();
        $recordButton.removeClass("Rec");
        $recordButton.addClass("notRec");
}

recognition.onnomatch = function (event) {
    diagnostic.textContent = "I didn't recognise that color.";
}

recognition.onerror = function (event) {
    diagnostic.textContent = 'Error occurred in recognition: ' + event.error;
}

//$('#recButton').addClass("notRec");

//$('#recButton').click(function () {
//    if ($('#recButton').hasClass('notRec')) {
//        $('#recButton').removeClass("notRec");
//        $('#recButton').addClass("Rec");
//    }
//    else {
//        $('#recButton').removeClass("Rec");
//        $('#recButton').addClass("notRec");
//    }
//});	