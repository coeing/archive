window.game = window.game || {};

window.game.audio = new function(){
	var path = 'assets/sounds/';

	function Sound(file) {
		var audio;
		if (!file.match(path)) {
			file = path + file;
		}
		audio = new Audio(file);

		this.play = function() {
			// stop & reset sound before playing again:
			audio.pause();
			audio.currentTime = 0;
			audio.play();

			return this;
		};
		this.stop = function() {
			audio.pause();

			return this;
		};
	}

	this.ingame = new Sound('Sound_IngameMusic.ogg');
	this.addpart = new Sound('sound_addpart.ogg');
	this.error =  new Sound('sound_error.ogg');

	this.russians = new Sound('Sound_USSR_Boarding.ogg');
	this.americans = new Sound('Sound_US_Boarding.ogg');
	return this;
}();


