window.game = window.game || {};

window.game.spritePath = 'assets/sprites/';

window.game.money = 0;

window.game.stageConfig = function() {

    this.distance = 2000;

    this.objectType = null;

    this.backgroundType = null;

    this.minSpawnCooldown = 0.5;
    this.maxSpawnCooldown = 1.5;

    this.minSpeed = {'x': 20.0, 'y': 1.0};
    this.maxSpeed = {'x': 50.0, 'y': 2.0};

    this.screenSideRatio = 0.2;
    this.minScreenPositionRatio = 0.5;
    this.maxScreenPositionRatio = 1.1;
}

window.game.state = function() {
    this.init = function() {

    }

    this.deinit = function() {

    }

    this.update = function(dt) {

    }
}

window.game.stateGame = function() {

}

window.game.engine = new (function() {
	var input = {
			'keysPressed': {}
		},
		gui = {
			'altitude': null,
			'fuelbar': null
		},
		that = this,
        rocket,
        rocketScreenPosition = {'x':0, 'y':0};

        var backgroundManager;

        this.__defineGetter__('backgroundManager', function() {
                return backgroundManager;
        });

        var enemyManager;

        this.__defineGetter__('enemyManager', function() {
                return enemyManager;
        });
        
        var isInited = false;

        this.__defineGetter__('isInited', function() {
                return isInited;
        });
        
        var isPaused = false;

        var stage = 0;

        var stageEnemyDistance = 4000;

        var stageConfigs = [];

        // Empty stage.
        var stageStart = new window.game.stageConfig();
        stageStart.distance = 1000;
        //stageStart.backgroundType = window.world.Cloud;
        stageConfigs.push(stageStart);

        var stageBirds = new window.game.stageConfig();

        stageBirds.distance = stageEnemyDistance;
        stageBirds.objectType = window.enemies.Bird;
        stageBirds.backgroundType = window.world.Cloud;
        stageBirds.minSpawnCooldown = 0.3;
        stageBirds.maxSpawnCooldown = 1.2;

        stageBirds.minSpeed = {'x': 20.0, 'y': 1.0};
        stageBirds.maxSpeed = {'x': 50.0, 'y': 2.0};

        stageBirds.screenSideRatio = 0.2;
        stageBirds.minScreenPositionRatio = 0.5;
        stageBirds.maxScreenPositionRatio = 1.1;

        stageConfigs.push(stageBirds);

        // Empty stage.
        var stageBirdsRelax = new window.game.stageConfig();
        stageBirdsRelax.backgroundType = window.world.Cloud;
        stageConfigs.push(stageBirdsRelax);

        var stageBalloons = new window.game.stageConfig();

        stageBalloons.distance = stageEnemyDistance;
        stageBalloons.objectType = window.enemies.Balloon;
        stageBalloons.backgroundType = window.world.Cloud;
        stageBalloons.minSpawnCooldown = 0.9;
        stageBalloons.maxSpawnCooldown = 1.5;

        stageBalloons.minSpeed = {'x': 5.0, 'y': 1.0};
        stageBalloons.maxSpeed = {'x': 10.0, 'y': 2.0};

        stageBalloons.screenSideRatio = 0.1;
        stageBalloons.minScreenPositionRatio = 0.9;
        stageBalloons.maxScreenPositionRatio = 1.3;

        stageConfigs.push(stageBalloons);

        // Empty stage.
        var stageBalloonsRelax = new window.game.stageConfig();
        stageBalloonsRelax.backgroundType = window.world.Cloud;
        stageConfigs.push(stageBalloonsRelax);

        // Satellites!
        var stageSatellites = new window.game.stageConfig();

        stageSatellites.distance = stageEnemyDistance;
        stageSatellites.objectType = window.enemies.Satellite;
        stageSatellites.backgroundType = null;
        stageSatellites.minSpawnCooldown = 1.5;
        stageSatellites.maxSpawnCooldown = 2.0;

        stageSatellites.minSpeed = {'x': 10.0, 'y': 5.0};
        stageSatellites.maxSpeed = {'x': 30.0, 'y': 10.0};

        stageSatellites.screenSideRatio = 0.2;
        stageSatellites.minScreenPositionRatio = 0.8;
        stageSatellites.maxScreenPositionRatio = 1.3;

        stageConfigs.push(stageSatellites);

        // Empty stage.
        var stageSatellitesRelax = new window.game.stageConfig();
        stageConfigs.push(stageSatellitesRelax);

        // Astroids!
        var stageAstroids = new window.game.stageConfig();

        stageAstroids.distance = stageEnemyDistance;
        stageAstroids.objectType = window.enemies.Astroid;
        stageAstroids.backgroundType = null;
        stageAstroids.minSpawnCooldown = 3.0;
        stageAstroids.maxSpawnCooldown = 3.5;

        stageAstroids.minSpeed = {'x': 10.0, 'y': 20.0};
        stageAstroids.maxSpeed = {'x': 20.0, 'y': 40.0};

        stageAstroids.screenSideRatio = 0.1;
        stageAstroids.minScreenPositionRatio = 1.1;
        stageAstroids.maxScreenPositionRatio = 1.3;

        stageConfigs.push(stageAstroids);

        // Empty stage.
        var stageAstroidsRelax = new window.game.stageConfig();
        stageConfigs.push(stageAstroidsRelax);

	this.init = function() {
		this.mibbu = new mibbu(window.game.conf.canvas.x, window.game.conf.canvas.y, 'wrapper');
		this.mibbu.init();

		gui.altitude = document.getElementById('altitude');
		gui.fuelbar = document.getElementById('fuel-bar');
		gui.lifebar = document.getElementById('life-bar');

		document.addEventListener('keydown', function(e) {
			input.keysPressed[e.keyCode] = e;
//			e.preventDefault();
		});

		document.addEventListener('keyup', function(e) {
			delete input.keysPressed[e.keyCode];
		});

		window.ROCKET = that.rocket = createRocket();

                enemyManager = new window.game.enemyManager();
                enemyManager.rocket = that.rocket;
                enemyManager.stageConfig = stageConfigs[stage];
                enemyManager.init();

                backgroundManager = new window.game.backgroundManager();
                // Black at satellites.
                backgroundManager.spaceDistance = 0;
                for (var i = 0; i < 5; i++)
                {
                    backgroundManager.spaceDistance += stageConfigs[i].distance; 
                }
                backgroundManager.rocket = that.rocket;
                backgroundManager.canvas = this.mibbu.canvas();
                backgroundManager.stageConfig = stageConfigs[stage];
                backgroundManager.init();

		var additionalLoop = function() {

                        var updateTime = 1/60.0;

                        // Speed up?
                        if (input.keysPressed[121]) {
				updateTime *= 10.0;
			}
            
            // Self destruct?
            if (input.keysPressed[35]) {
                that.rocket.destroy();
            }
            
            // Give money?
            if (input.keysPressed[77]){
                window.game.money += 1000;
                if (window.ROCKETEDITOR)
                {
                    window.ROCKETEDITOR.updateMoneyDisplay();
                }
                input.keysPressed[77] = false;
            }
            
            // Pause?
            if (input.keysPressed[80])
            {
                that.isPaused = !that.isPaused;
                input.keysPressed[80] = false;
            }
            
            // God Mode?
            if (input.keysPressed[71] &&    // 'G'
                input.keysPressed[79] &&    // 'O'
                input.keysPressed[68])      // 'D'
            {
                that.enemyManager.godMode = !that.enemyManager.godMode;
                input.keysPressed[71] = false;
                input.keysPressed[79] = false;
                input.keysPressed[68] = false;
            }

			// Handle input.
			if (window.game.screens.current_screen === 'scoreboard' && input.keysPressed[32]) {
				window.game.screens.activate_editor();
                input.keysPressed[32] = false;
			}

            if (that.isPaused)
            {
                that.ctx().textAlign = 'center';
                that.ctx().font = '25px VT323';
                that.ctx().fillText('Pause', 250, 200);
                return;
            }
            else if (window.ROCKETEDITOR)
            {
                that.ctx().textAlign = 'center';
                that.ctx().font = '25px VT323';
                that.ctx().fillText('Press SPACE', 250, 160); 
                that.ctx().fillText('to exit editor', 250, 200);                
            }
            else if (!that.rocket.isEngineStarted())
            {
                that.ctx().textAlign = 'center';
                that.ctx().font = '25px VT323';
                that.ctx().fillText('Press SPACE to lift off!', 250, 200);                
            }
            else if (that.enemyManager.godMode)
            {
                that.ctx().textAlign = 'center';
                that.ctx().font = '25px VT323';
                that.ctx().fillText('I believe I can fly!', 250, 200);                
            }

            var pressedStart = input.keysPressed[83] || input.keysPressed[32];
            if (pressedStart) {
                // First remove editor and start countdown.
                if (window.ROCKETEDITOR && window.ROCKETEDITOR.close()) {
                    delete window.ROCKETEDITOR;
                } else {
                    that.rocket.startEngine();
                }
                input.keysPressed[83] = false;
                input.keysPressed[32] = false;
            }

            gui.altitude.innerHTML = ~~that.rocket.altitude;
            gui.fuelbar.style.width = (((that.rocket.fuel / that.rocket.fuelcap * 158)>>1)<<1) + 'px';
            gui.lifebar.style.width = (((that.rocket.life_points / that.rocket.life_points_max * 158)>>1)<<1) + 'px';

            try
            {
                if (that.rocket.canMove())
                {
                    that.rocket.fly(input, updateTime);
                }
            }
            catch (e)
            {
                debug("Exception " + e);
                that.rocket.destroy();
            }

            enemyManager.update(updateTime);
            backgroundManager.update(updateTime);

            // Check if stage changed.
            updateStage();

            if (that.rocket.flyHeight > 100 &&
                window.game.screens.current_screen !== 'scoreboard' &&
                !window.game.screens.hudVisible)
            {
                window.game.screens.showHUD(true);
            }
		}

		//now add that function to the loop
		//and start checking for the collisions
		this.mibbu.on().hook(additionalLoop);
        
        isInited = true;
	};
    
    this.reset = function reset() {
        
        // Create new rocket.
        window.ROCKET = that.rocket = createRocket();
        
        // Assign new rocket to managers.
        that.enemyManager.rocket = that.rocket;
        that.backgroundManager.rocket = that.rocket;        
        
        updateStage();
    }
    
    function createRocket() {
		var rocket = new window.rocket.Rocket();
		rocket.pos = {
			'x' : window.game.conf.canvas.x / 2,
			'y' : 0
		};
        rocket.init();

        rocketScreenPosition.x = ( window.game.conf.canvas.x - rocket.width ) / 2;
        rocketScreenPosition.y = ( window.game.conf.canvas.y - rocket.height );
		window.sprites = window.game.renderRocket(rocket, rocketScreenPosition)
//		window.sprites = window.game.renderRocket(rocket);

        return rocket;
    }
    
    function updateStage() {        
        var newStage = computeStage();
        if (newStage != that.stage)
        {
            debug("New stage " + newStage);
            that.stage = newStage;
            backgroundManager.stageConfig = stageConfigs[that.stage];
            enemyManager.stageConfig = stageConfigs[that.stage];
        }
    }

    function computeStage() {
        var flightHeight = that.rocket.flyHeight;

        var distance = 0;
        for (var i = 0; i < stageConfigs.length; i++)
        {
            distance += stageConfigs[i].distance;
            if (flightHeight < distance)
            {
                return i;
            }
        }

        return stageConfigs.length - 1;
    }
    
    this.ctx = function() {
        return that.mibbu.ctx();
    }

	return this;
})();

window.game.screens = new (function() {
	var current_screen = 'start';
    
    var hudVisible = false;

	this.init = function init() {
		this.activate_start();
		debug(this);
	};

	this.activate_editor = function activate_editor() {
		this.hide_current_screen();
        if (!window.game.engine.isInited)
        {
            window.game.engine.init();
        }
        else
        {
            window.game.engine.reset();
        }
		window.ROCKETEDITOR = new window.RocketEditor();
		this.show('editor');
	};

	this.activate_start = function activate_start() {
		this.hide_current_screen();
		this.show('start');
		window.game.current_screen = new StartScreen();
	};

	this.activate_scoreboard = function activate_scoreboard() {
		var element = this.show('scoreboard');
		element.addClass(window.game.faction);

        this.showHUD(false);

		document.getElementById('altitude_score').innerHTML = ROCKET.altitude;
		var money_reward = ~~ROCKET.altitude;
		debug(window.game.money);
		debug(ROCKET.cost);
		window.game.money = window.game.money + money_reward;
		document.getElementById('money_score').innerHTML = money_reward;
	};

	this.hide_current_screen = function hide_current_screen() {
		this.hide(current_screen);
	};

	this.show = function show(screen_id) {
		debug('show: ' + screen_id);
		var element = document.getElementById(screen_id);
		element.setAttribute('style', 'display:block');
		current_screen = screen_id;
		return element;
	};

	this.hide = function hide(screen_id) {
		debug('hide: ' + screen_id);
		var element = document.getElementById(screen_id);
		element.setAttribute('style', 'display:none');
		return element;
	};

	this.__defineGetter__('current_screen', function() {
		return current_screen;
	});
	
	this.__defineGetter__('hudVisible', function() {
		return hudVisible;
	});
    
    this.showHUD = function showHUD(show) {
        
        if (show == hudVisible)
        {
            return;
        }
        
        if (show)
        {
            // Show GUI.
            fadein('info-bars');
            fadein('altitude');
            //document.getElementById('info-bars').style.display = 'block';
            //document.getElementById('altitude').style.display = 'block';
        }
        else
        {
            // Hide GUI.
            document.getElementById('info-bars').setAttribute('style', 'display:none');
            document.getElementById('altitude').setAttribute('style', 'display:none');
        }
        
        hudVisible = show;
    }

	return this;
})();
