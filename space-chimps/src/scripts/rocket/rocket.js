window.rocket = {};

window.game = window.game || {};

window.rocket.Rocket = function Rocket() {
	var parts = [],
		that = this,
		height = 0,
		width = 58,
		cost,
		defaultparts = {
			'capsule': new window.rocket.Capsule(),
			'fuel_tank': new window.rocket.FuelTank(),
			'engine': new window.rocket.Engine()
		},
		position = {
			'x': 150,
			'y': 150
		},
		life_points_max = 100,
		life_points = 0,
		// motion in x- or y-axis
		acc = {
			'x': 0,
			'y': 0
		},
		vector = {
			'x': 0,
			'y': 0
		},
		sprites = {},
		conf = window.game.keyconf,
		half_width = width / 2,
		animate_rct = false,
		collider;

		this.init = function init() {
			var i = this.parts.length,
				part;

			while (i--) {
				part = this.parts[i];

				height += part.height;
//				weight += part.weight;
				cost *= part.cost
				life_points_max += part.life_points || 0;

				if (part.type == window.rocket.ReactionControlThruster.type) {
					animate_rct = true;
				}

//				fuel_factor *= part.fuel_factor || 1
			}
			life_points = life_points_max;

                    // Create rocket collider.
                    collider = new window.collision.Collider();

                    var afterburnerHeight = 62;
                    var capsuleRadius = 29;
                    var rectHeight = that.height - afterburnerHeight - capsuleRadius;
                    var shape = new window.collision.RectShape(width, rectHeight);
                    shape.offset.y = (afterburnerHeight - capsuleRadius) / 2;
                    collider.shapes.push(shape);

                    var circleShape = new window.collision.CircleShape(capsuleRadius);
                    circleShape.offset.y = that.height / 2 - capsuleRadius;
                    collider.shapes.push(circleShape);
		};

                var dragCoefficient = 2.0;
                var dragCoefficientSide = 20.0;

		/**
		 * fly like an eagle.
		 */
		this.fly = function fly(input, dt) {
			var i, j,
				keys = input.keysPressed,
				len = parts.length,
				part,
				pos;

			// loop through user input
                        var steerSide = 0;
                        var enableBoost = 0;
			for (j in keys) {
				// loop through parts
//				for (i = 0; i < len; i++) {
//					part = parts[i];
//					// if key for part pressed, trigger action
//					if (conf[keys[j]] === part.type) {
//						part.use();
//					}
//				}

				if (this.canSteer()) {
					if (j == 37) {
                                            steerSide = -1;
						if (animate_rct) {

						}
					} else if (j == 39) {
                                            steerSide = 1;
						if (animate_rct) {

						}
					}
					if (j == 38) {
                                            enableBoost = 1;
                                        }
				}
			}

                        var minDrag = 20000;
                        var minDragSide = 20000;
                        var sideThrust = 10000 * this.agility;
                        var force = 0;
                        var sideForce = 0;
                        var throttle = defaultparts.engine.started && !defaultparts.fuel_tank.empty();
                        if (throttle)
                        {
                            force = defaultparts.engine.thrust;
                            if (enableBoost)
                            {
                                force *= defaultparts.engine.boostFactor;
                            }

                            sideForce = sideThrust;
                        }

                        var drag = vector.y * vector.y * dragCoefficient;
                        // Minimum drag to reach zero.
                        if (!throttle && drag < minDrag)
                        {
                            drag = minDrag;
                        }
                        acc.y = (force - drag) / that.weight;

                        var dragDir = vector.x > 0 ? 1 : -1;
                        var dragSide = vector.x * vector.x * dragCoefficientSide;

						if (isNaN(vector.x)) {
							debugger
						}

                        // Minimum drag to reach zero.
                        if (steerSide == 0 &&
                            dragSide < minDragSide)
                        {
                            dragSide = minDragSide;
                        }
                        dragSide *= dragDir;

                        if (steerSide == -1) {
                                acc.x = (-sideForce - dragSide) / that.weight;
                        } else if (steerSide == 1) {
                                acc.x = (sideForce - dragSide) / that.weight;
                        }
                        else {
                            acc.x = -dragSide / that.weight;
                        }

                        // Make sure to not flip speed if no thrust.
                        var speedDelta = acc.x * dt;
                        if (steerSide == 0 &&
                            Math.abs(speedDelta) > Math.abs(vector.x))
                        {
                            speedDelta = -vector.x;
                        }
						vector.x += speedDelta;
                        var speedDeltaY = acc.y * dt;
                        if (!throttle &&
                            Math.abs(speedDeltaY) > Math.abs(vector.y))
                        {
                            speedDeltaY = -vector.y;
                        }
			vector.y += speedDeltaY;

			// clip
			if (position.x < half_width) {
				vector.x = -vector.x;
				acc.x = -acc.x;
				position.x = half_width;
				// play sound
			} else if (position.x + half_width >= window.game.conf.canvas.x) {
				vector.x = -vector.x;
				acc.x = -acc.x;
				position.x = window.game.conf.canvas.x - half_width;
				// play sound
			}
			position.x += vector.x * dt;
            var distanceY = vector.y * dt;
            position.y += distanceY;

            // Apply new rocket position.
            that.collider.pos = position;

            if (!defaultparts.fuel_tank.empty()) {
                // use engine, burn fuel
                defaultparts.engine.use(defaultparts.fuel_tank, this.fuel_factor, distanceY);

                // Check if tank became empty.
                if (defaultparts.fuel_tank.empty() || !defaultparts.engine.started) {
                    defaultparts.engine.currentAnim = 0;
                }
                else if (enableBoost) {
                    defaultparts.engine.currentAnim = 2;
                }
                else {
                    defaultparts.engine.currentAnim = 1;
                }

                if (defaultparts.engine.currentAnim !== window.sprites[defaultparts.engine.type].animation()) {
                    window.sprites[defaultparts.engine.type].animation(defaultparts.engine.currentAnim);
                }
            }

			for (j in window.sprites) {
				window.sprites[j].position(~~this.screenpos.x);
			}

			// Check if no more movement possible.
			if (!this.canMove())
			{
			    throw 'NoMoreFuelAndSpeed'
			}
		};

		this.canSteer = function() {
			return !defaultparts.fuel_tank.empty() && defaultparts.engine.started;
		};

		this.canMove = function() {
			return !defaultparts.fuel_tank.empty() || vector.y > 0;
		};

		this.startEngine = function() {
			if (defaultparts.engine.started) {
				return;
			}

			defaultparts.engine.start();

			if (defaultparts.engine.currentAnim !== window.sprites[defaultparts.engine.type].animation()) {
				window.sprites[defaultparts.engine.type].animation(defaultparts.engine.currentAnim);
			}
		};

		this.isEngineStarted = function() {
			return defaultparts.engine.started;
		};

		this.stopEngine = function() {
			defaultparts.engine.stop();

			if (defaultparts.engine.currentAnim !== window.sprites.engine.animation()) {
				window.sprites.engine.animation(defaultparts.engine.currentAnim);
			}
		}

		this.addPart = function addPart(part) {
			var i;

			if (part.is_upgrade) {
				// search for old part

				for (i in defaultparts) {
					if (defaultparts[i].constructor == part.is_upgrade)  {
						this.removePart(defaultparts[i]);
						defaultparts[i] = part;
						break;
					}
					if (defaultparts[i].type === part.type) {
						return false;
					}
				}
			} else {
				i = parts.length;
				while (i--) {
					if (parts[i].type === part.type) {
						return false;
					}
				}
				parts.push(part);
			}

			return true;
		};

		this.hasPart = function(part) {

			var i;
			if (part.is_upgrade) {
				// Check if part replaced default part.
				for (i in defaultparts) {
					if (defaultparts[i] === part)  {
						return true;
					}
				}
			} else {
				i = parts.length;
				while (i--) {
					if (parts[i].type === part.type) {
						return true;
					}
				}
			}

			return false;
		};

		this.removePart = function removePart(part) {
			var i, j;
			debug('remove');


			i = parts.length;
			// hide old sprite
			if (sprites[part.type]) {

			}

			while (i--) {
				if (parts[i].type == part.type) {
					return parts.remove(i);
				}
			}
			for (i in defaultparts) {
				if (defaultparts[i].type == part.type) {
					for (j in sprites) {
						if (j === defaultparts[i].type) {
						debug(defaultparts[i].type)
							sprites[j].remove();

						}
					}
					if (part.is_upgrade) {
						defaultparts[i] = new part.is_upgrade();

					}
					break;
				}
			}
		};

		this.destroy = function destroy() {
			debug('Rocket...destroyed. What a pity =( .');

			// Trigger explosion, end round.
			var maxOffsetX = 50,
				offset = 0,
				explosions = [];

			this.stop();


			while (offset < this.height) {
			    var explosion = new window.world.Explosion();
			    explosion.spritePosition.x = this.pos.x - explosion.width / 2 + maxOffsetX / 2 - Math.random() * maxOffsetX;
			    explosion.spritePosition.y = window.game.conf.canvas.y - (explosion.height + this.height /2 - offset);
			    explosion.spritePosition.z = 30.0;
			    explosion.init();

				explosions.push(explosion);

			    offset += explosion.height / 3;
			}
			window.setTimeout(function(){
				that.onRocketExplosionOver(explosions);
			}, 250);

			// Hide rocket.
			for (var i in this.sprites) {
				this.sprites[i].remove();
			}
		};

		this.stop = function stop() {
			defaultparts.engine.stop()
			acc.x = 0;
			acc.y = 0;
			vector.x = 0;
			vector.y = 0;
		}

		this.onRocketExplosionOver = function onRocketExplosionOver(explosions) {
			var i = explosions.length;
			while (i--) {
				explosions[i].sprite.remove();
			}

			// Move sprite to position outside the screen.
			this.position = {
				'x': 1000,
				'y': 1000
			};

			window.game.screens.activate_scoreboard();
		}

		this.__defineGetter__('parts',function() {
			var ret = [defaultparts.capsule];

			ret = ret.concat(parts, defaultparts.fuel_tank, defaultparts.engine);

			return ret.reverse();
		});

		this.__defineGetter__('screenpos', function() {
			return {
				'x': position.x - half_width,
				'y': window.game.conf.canvas.y - that.height
			}
		});

		this.__defineGetter__('pos', function() {
			return position;
		});

		this.__defineSetter__('pos', function(pos) {
			position = pos;
		});

		this.__defineGetter__('width', function() {
			return width;
		});

		this.__defineGetter__('height', function() {
			var _height = 0,
				i = that.parts.length;

			while (i--) {
				_height += that.parts[i] ? that.parts[i].height : 0;
			}

			return height = _height;
		});

		//
		this.__defineGetter__('weight', function() {
			var _weight = 0,
				i = that.parts.length;

			while (i--) {
				_weight += that.parts[i].weight;
			}

			return _weight;
		});

		//
		this.__defineGetter__('cost', function() {
			var _cost = 0,
				i = that.parts.length;

			while (i--) {
				debug(that.parts[i])
				_cost += that.parts[i].cost;
			}

			return cost = _cost;
		});

		this.__defineGetter__('fuel_factor', function() {
			var factor = 1,
				i = that.parts.length;

			while (i--) {
				factor *= that.parts[i].fuel_factor || 1;
			}

			return factor;
		});


		this.__defineGetter__('fuel', function() {
			var _fuel = 0,
				i = that.parts.length;

			while (i--) {
				_fuel += that.parts[i].fuel || 0;
			}

			return _fuel;
		});

		this.__defineGetter__('fuelcap', function() {
			var _fuel = 0,
				i = that.parts.length;

			while (i--) {
				_fuel += that.parts[i].fuel_capacity || 0;
			}

			return _fuel;
		});

		this.__defineGetter__('life_points', function() {
			return life_points;
		});

		this.__defineSetter__('life_points', function(_life_points) {
			life_points = Math.max(_life_points, 0);
		});

		this.__defineGetter__('life_points_max', function() {
			return life_points_max;
		});

		this.__defineGetter__('agility', function() {
			var _agility = 1,
				i = that.parts.length;

			while (i--) {
				_agility *= that.parts[i].agility || 1;
			}

			return _agility;
		});

		//
		this.__defineGetter__('sprites', function() {
			return sprites;
		});
		this.__defineSetter__('sprites', function(_sprites) {
			sprites = _sprites;
		});

		//
                this.__defineGetter__('flyHeight', function() {
                    var flyHeight = this.pos.y - this.height / 2;
                    return flyHeight;
		});

		//
		this.__defineGetter__('collider', function() {
			return collider;
		});

		this.__defineGetter__('altitude', function() {
			return ~~position.y;
		});
};


window.rocket.Capsule = function Capsule(){
	debug('constructor')
	debug(window.game.conf.scale)
	debug(this.width)
};
window.rocket.Capsule.inherits(window.abstractrocket.AbstractCapsule);
window.rocket.Capsule.prototype.type = 'capsule';
window.rocket.Capsule.prototype.is_base = true;
window.rocket.Capsule.prototype.width = 29;
window.rocket.Capsule.prototype.height = 22;
window.rocket.Capsule.prototype.frames = 1;
window.rocket.Capsule.prototype.anim = 1;
window.rocket.Capsule.prototype.animSpeed = 30;

window.rocket.HeavyMetalCapsule = function HeavyMetalCapsule() {
	this.frames = 1;
	this.anim   = 0;
	this.weight = 1500;
	this.life_points = 160;
	this.cost   = 6000;
	this.is_upgrade = window.rocket.Capsule;
	this.descr = 'A little health, a lot of weight!';
};

window.rocket.HeavyMetalCapsule.inherits(window.abstractrocket.AbstractCapsule);
window.rocket.HeavyMetalCapsule.prototype.type = 'heavymetalcapsule';
window.rocket.HeavyMetalCapsule.prototype.is_base = false;


/**
 *
 * fuel tank
 */
window.rocket.FuelTank = function FuelTank() {
	this.weight = 500;//current_fuel + 500;
	this.cost   = 0;
};
window.rocket.FuelTank.inherits(window.abstractrocket.AbstractFuelTank);
window.rocket.FuelTank.prototype.type = 'fueltank';
window.rocket.FuelTank.prototype.is_base = true;
window.rocket.FuelTank.prototype.fuel = 37000;
window.rocket.FuelTank.prototype.fuel_capacity = 37000;
window.rocket.FuelTank.prototype.width = 29;
window.rocket.FuelTank.prototype.height = 31;
window.rocket.FuelTank.prototype.frames = 1;
window.rocket.FuelTank.prototype.anim = 0;

window.rocket.AcidFuel = function AcidFuel() {
	this.cost = 4000;
	this.life_points = -50;
	this.descr = 'Doubled capacity, more health risk.'
	this.is_upgrade = window.rocket.FuelTank;
};
window.rocket.AcidFuel.inherits(window.abstractrocket.AbstractFuelTank);
window.rocket.AcidFuel.prototype.type = 'acidfuel';
window.rocket.AcidFuel.prototype.is_base = false;
window.rocket.AcidFuel.prototype.fuel = 50000;
window.rocket.AcidFuel.prototype.fuel_capacity = 50000;
window.rocket.AcidFuel.prototype.width = 29;
window.rocket.AcidFuel.prototype.height = 31;
window.rocket.AcidFuel.prototype.frames = 1;
window.rocket.AcidFuel.prototype.anim = 0;

window.rocket.HyperFuel = function HyperFuel() {
	this.cost   = 6000;
	this.weight = 150;
	this.height = 12;
	this.width  = 29;
	this.frames = 1;
	this.anim   = 0;
	this.descr  = 'It\'s super efficient, %1!';
	this.fuel_factor = 0.75;
};

window.rocket.HyperFuel.inherits(window.abstractrocket.RocketPart);
window.rocket.HyperFuel.prototype.type = 'hyperfuel';



window.rocket.ReactionControlThruster = function ReactionControlThruster() {
	this.cost   = 6000;
	this.weight = 200;
	this.height = 14;
	this.width  = 29;
	this.frames = 1;
	this.anim   = 0;
	this.descr  = 'Increases agility by over 9000, but needs fuel.';
	this.agility = 20;
	this.fuel_factor = 2.0;
};

window.rocket.ReactionControlThruster.inherits(window.abstractrocket.RocketPart);
window.rocket.ReactionControlThruster.prototype.type = 'reactioncontrolthruster';


/**
 *
 * raw power.
 *
 */
window.rocket.Engine = function Engine() {
	this.cost    = 0;
	this.width   = 29;
	this.weight  = 250;
	this.height  = 54;
	this.frames  = 1;
	this.anim    = 2;
	this.speed   = 15;
	this.started = false;

	// Thrust.
	this.thrust = 50000.0;

	// Factor by which the acceleration is boosted when the boost is active.
	this.boostFactor = 5.0;
};
window.rocket.Engine.inherits(window.abstractrocket.AbstractEngine);
window.rocket.Engine.prototype.type = 'engine';
window.rocket.Engine.prototype.is_base = true;



window.rocket.NuclearEngine = function NuclearEngine() {
	this.cost = 4000;

    // Factor by which the acceleration is boosted when the boost is active.
    this.boostFactor = 10.0;

	this.descr  = 'Warp speed, Mr Sulu!';
	this.is_upgrade = window.rocket.Engine
};

window.rocket.NuclearEngine.inherits(window.abstractrocket.AbstractEngine);
window.rocket.NuclearEngine.prototype.type = 'nuclearengine';
window.rocket.NuclearEngine.prototype.is_base = false;

window.rocket.Engine = function Engine(){
	this.cost    = 0;
//	this.width   = 29;
	this.weight  = 250;
//	this.height  = 54;
//	this.frames  = 1;
//	this.anim    = 1;
};
window.rocket.Engine.inherits(window.abstractrocket.AbstractEngine);
window.rocket.Engine.prototype.type = 'engine';
window.rocket.Engine.prototype.is_base = true;


/*
window.rocket.DummyPart = function DummyPart() {
	this.cost = 100;
	this.height = 11;
	this.width  = 29;
	this.weight = 1000;
	this.frames = 1;
	this.anim   = 0;

};
window.rocket.DummyPart.inherits(window.abstractrocket.RocketPart);
window.rocket.DummyPart.prototype.type = 'dummy';

window.rocket.DummyPart2 = function DummyPart2() {
	this.cost = 200;
	this.height = 11;
	this.width  = 29;
	this.weight = 1000;
	this.frames = 1;
	this.anim   = 0;

};
window.rocket.DummyPart2.inherits(window.abstractrocket.RocketPart);
window.rocket.DummyPart2.prototype.type = 'dummy2';
*/
