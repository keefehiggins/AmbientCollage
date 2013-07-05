AmbientCollage = AmbientCollage || {};

var Global = {
    CurrentlyPlaying: [],
    CurrentExperience: {},
    StopAllSounds: function(){
        for (var i = 0; i < currentlyPlaying.length; i++) {
            currentlyPlaying[i].stop();
        }
    },
    LoadExperiences: function (currentUserId, onlyMine, fillTarget) {
        $.ajax({
            url: "LoadUserExperiences",
            cache: false,
            data: {
                userId: currentUserId || -1,
                onlyMine: onlyMine ? onlyMine : false
            },
            method: "GET",
            success: function (data) {
                $(fillTarget).html(data);
            }
        });
    },
    SetBackground: function (imageUrl, callBack) {
        $('#centerContent').stop().animate(
               { opacity: "0" },
               500,
               function () {
                   $('#centerContent').css('background-image', 'url(' + imageUrl + ')').stop().animate(
                           { opacity: "1" },
                           500,
                           function () {
                               callBack = callBack || {};
                               callBack();
                           }
                       );
               }
           );
    },
    PlayAudioTrack: function (trackUrl, volume, loops){
        var audioLink = '/tracks/' + trackUrl;

        SC.stream(audioLink, function (sound) {
            sound.play({
                //onfinish: stopAllSounds(),
                loops: loops,
                volume: volume
            });
            currentlyPlaying.push(sound);
        });
    },
    StopAllSounds: function () {
    },
    BeginExperience: function (backgroundUrl, audioList) {
    }
}

