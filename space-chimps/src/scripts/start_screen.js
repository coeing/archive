window.StartScreen = function StartScreen() {
	var that = this;

	this.init = function init() {
		var element = document.getElementById('russians');
		element.addEventListener('click', function() {that.choose_russians()});
		element = document.getElementById('americans');
		element.addEventListener('click', function() {that.choose_americans()});
	}

	this.choose_russians = function choose_russians() {
		this.choose_faction('russians');
		window.game.screens.activate_editor();
	};

	this.choose_americans = function choose_americans() {
		this.choose_faction('americans');
		window.game.screens.activate_editor();
	};

	this.choose_faction = function choose_faction(faction) {
		window.game.audio[faction].play();
		window.game.faction = faction;
	}

	this.init();
};
