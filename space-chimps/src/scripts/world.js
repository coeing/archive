window.world = {}

window.world.SpriteObject = function SpriteObject() {

	var width = 0;
	var height = 0;
    
    // Number of frames per animation.
	var frames = 1;
    
    // Number of animations in the sprite.
	var anim = 0;
    
    // Speed the animation should be played with.
	var animSpeed = 0;
    
    // Current animation.
	var currentAnim = 0;

	var spriteOffset = {
		'x': 0,
		'y': 0
	};

	var sprite;

	var speed = {
		'x': 0,
		'y': 0
	};

	var spritePosition = {
		'x': 0,
		'y': 0,
		'z': 0
	};

    this.init = function init() {
        sprite = window.game.renderEnemy(this);
    }

    this.deinit = function deinit() {
        sprite.remove();
    };


    this.update = function update(dt) {
		// Update position.
		spritePosition.x += speed.x * dt;
		spritePosition.y += speed.y * dt;

    };

    // width
    this.__defineGetter__('width', function() {
            return width * window.game.conf.scale;
    });
    this.__defineSetter__('width', function(_width) {
            width = _width;
    });

    // height
    this.__defineGetter__('height', function() {
            return height * window.game.conf.scale;
    });
    this.__defineSetter__('height', function(_height) {
            height = _height;
    });

    // part sprite
    this.__defineGetter__('image', function() {
            return window.game.spritePath + this.type + '.png';
    });

    // frames
    this.__defineGetter__('frames', function() {
            return frames;
    });
    this.__defineSetter__('frames', function(_frames) {
            frames = _frames;
    });

    // animations
    this.__defineGetter__('anim', function() {
            return frames ? anim : 0;
    });
    this.__defineSetter__('anim', function(_anim) {
            anim = _anim;
    });
    
    this.__defineGetter__('currentAnim', function() {
            return currentAnim;
    });
    this.__defineSetter__('currentAnim', function(_value) {
            currentAnim = _value;
            if (sprite) {
                sprite.animation(currentAnim);
            }
    });

    this.__defineGetter__('animSpeed', function() {
            return animSpeed;
    });
    this.__defineSetter__('animSpeed', function(_value) {
            animSpeed = _value;
    });

    this.__defineGetter__('sprite', function() {
            return sprite;
    });
    this.__defineSetter__('sprite', function(_value) {
            sprite = _value;
    });

    this.__defineGetter__('spriteOffset', function() {
            return spriteOffset;
    });

    this.__defineGetter__('speed', function() {
            return speed;
    });
    this.__defineSetter__('speed', function(_value) {
            speed = _value;
    });

    this.__defineGetter__('spritePosition', function() {
            return spritePosition;
    });
    this.__defineSetter__('spritePosition', function(_value) {
            spritePosition = _value;
    });
}

window.world.Cloud = function Cloud() {
    window.world.SpriteObject.call(this);

    this.width = 83;
    this.height = 26;
    this.frames = 1;
    this.animSpeed = 0;
}

//window.world.Cloud.inherits(window.world.SpriteObject);
window.world.Cloud.prototype.type = 'sprite_cloud_small';

window.world.Explosion = function Explosion() {
    window.world.SpriteObject.call(this);

	this.width = 87;
    this.height = 81;
    this.frames = 7;
    this.animSpeed = 5;
}

window.world.Explosion.prototype.type = 'sprite_explosion';


window.world.SmallExplosion = function SmallExplosion() {
    window.world.SpriteObject.call(this);

	this.width = 32;
	this.height = 31;
	this.frames = 3;
	this.animSpeed = 5;
}

window.world.SmallExplosion.prototype.type = 'sprite_explosion_small';

window.world.BirdExplosion = function BirdExplosion() {
    window.world.SpriteObject.call(this);

	this.width = 30;
	this.height = 30;
	this.frames = 3;
	this.animSpeed = 5;
}

window.world.BirdExplosion.prototype.type = 'sprite_explosion_bird';
