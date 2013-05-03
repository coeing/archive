window.abstractrocket = window.abstractrocket || {};

window.abstractrocket.RocketPart = function RocketPart() {

	var width = 0;
	var height = 0;
	var frames = 1;
	var anim = 0;
	var animSpeed = 0;

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
};

//	var width = 0;
//	var height = 0;
//	var frames = 1;
//	var anim = 0;
//	var animSpeed = 0;
window.abstractrocket.RocketPart.prototype = {
		'animSpeed': 0,
//		'sprite': null,
		'spriteOffset': {
			'x': 0,
			'y': 0
		},
		'speed': {
			'x': 0,
			'y': 0
		},
		'spritePosition': {
			'x': 0,
			'y': 0,
			'z': 0
		},



//		'init': function init() {
//			this.sprite = window.game.renderEnemy(this);
//		},
//		'deinit': function deinit() {
//			this.sprite.remove();
//		},
//		'update': function update(dt) {
//			// Update position.
//			debug(this)
//			debugger
//			this.spritePosition.x += speed.x * dt;
//			this.spritePosition.y += speed.y * dt;
//
//		},
		//-----------------------------
		get sprite() {
			return this.__sprite;
		},

		set sprite(sprite) {
			this.__sprite = sprite;
		},

		// weight
		get weight() {
			return this.__weight;
		},

		set weight(_weight) {
			this.__weight = _weight;
		},

		// width
		get width() {
			console.log('-get-')
			console.log(this.__width)
			console.log(window.game.conf.scale)
			console.log(this)
			console.log('-----')
//			var foo = this.__width * window.game.conf.scale;
			var foo = this.__width * 2;
			return foo;
		},
		set width(width) {
			this.__width = width;
		},

		// height
		get height() {
			return this.__height * window.game.conf.scale;
		},
		set height(height) {
			this.__height = height;
		},

		// part sprite
		get image() {
			return window.game.spritePath + this.type + '.png';
		},

		// frames
		get frames() {
			return this.__frames;
		},
		set frames(frames) {
			this.__frames = frames;
		},

		// animations
		get anim() {
			return this.__frames ? this.__anim : 0;
		},
		set anim(anim) {
			this.__anim = anim;
		},

		// agility
		get agility() {
			return this.__agility || 1;
		},
		set agility(agility) {
			this.__agility = agility;
		},
		// part effect
		get effect() {
			return this.__effect;
		},
		set effect(effect) {
			this.__effect = effect;
		},
		// description
		get descr() {
			var str = 'dude';
			if (window.game.faction == 'russians') {
				str = 'comrade'
			}
			return this.__description.replace(/%1/, str);
		},
		set descr(descr) {
			this.__description = descr;
		},

		// cost
		get cost() {
			return this.__cost || 0;
		},
		set cost(cost) {
			this.__cost = cost
		}
	};

//window.abstractrocket.RocketPart.inherits(window.world.SpriteObject);
window.abstractrocket.RocketPart.prototype.use = function use() {
	if (window.debug_enabled) {
		debug(this.effect);
	}

	if (typeof this.effect == 'function') {
		this.effect.apply(this, arguments);
	}
};

/**
 *
 * command capsule
 */
window.abstractrocket.AbstractCapsule = function AbstractCapsule () {
	this.weight = 500;
};

window.abstractrocket.AbstractCapsule.inherits(window.abstractrocket.RocketPart);
window.abstractrocket.AbstractCapsule.prototype.type = 'capsule';

/**
 *
 * fuel tank
 */
window.abstractrocket.AbstractFuelTank = function AbstractFuelTank() {
//	window.abstractrocket.RocketPart.call(this);

	var fuel_capacity = 10000,
		current_fuel = 10000,
		burnt_fuel   = 1,
		factor       = 0,
		that = this;

	this.weight = 500;//current_fuel + 500;
	this.cost   = 0;

	// burn fuel
	this.effect = function(_factor, distance) {
		if (!current_fuel) {
			throw 'NoMoreFuel';
		}

		factor = _factor;
		current_fuel -= burnt_fuel * distance * _factor;

		if (window.debug_enabled) {
			debug('fuel: ' + current_fuel);
		}
	};

	this.empty = function() {
		return that.fuel <= 0;
	}

	this.__defineGetter__('fuel', function() {
		return current_fuel;
	});
	this.__defineSetter__('fuel', function(_fuel) {
		current_fuel = _fuel;
	});

	this.__defineSetter__('fuel_capacity', function(_cap) {
		fuel_capacity = _cap;
	});
	this.__defineGetter__('fuel_capacity', function() {
		return fuel_capacity;
	});
};
window.abstractrocket.AbstractFuelTank.inherits(window.abstractrocket.RocketPart);
window.abstractrocket.AbstractFuelTank.prototype.type = 'fueltank';

/**
 *
 * raw power.
 *
 */
window.abstractrocket.AbstractEngine = function AbstractEngine() {

	window.abstractrocket.RocketPart.call(this);

	this.cost    = 0;
	this.width   = 29;
	this.weight  = 250;
	this.height  = 54;
	this.frames  = 1;
	this.anim    = 1;
    this.currentAnim = 0;
	this.animSpeed   = 15;
	this.started = false;

        // Thrust.
        this.thrust = 50000.0;

        // Factor by which the acceleration is boosted when the boost is active.
        this.boostFactor = 5.0;

	this.effect = function(fuel_tank, factor, distance) {
		if (this.started) {
			try {
				fuel_tank.use(factor, distance);
			} catch (e) {}
		}
	};

	this.start = function() {
		this.started = true;
		this.currentAnim = 1;
	};

	this.stop = function() {
		this.started = false;
		this.currentAnim = 0;
	};
};

window.abstractrocket.AbstractEngine.inherits(window.abstractrocket.RocketPart);
window.abstractrocket.AbstractEngine.prototype.type = 'engine';