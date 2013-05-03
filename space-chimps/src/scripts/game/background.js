

window.game.backgroundManager = function() {
    var rocket;
    var canvas;
    var startBackgroundColor = {'r': 190, 'g': 220, 'b': 230, 'a': 1.0};
    var targetBackgroundColor = {'r': 45, 'g': 55, 'b': 100, 'a': 1.0};
    var colorDiff = {};
    var backgroundColor;
    var backgroundObjects = [];

    this.__defineGetter__('rocket', function() {
            return rocket;
    });
    this.__defineSetter__('rocket', function(_value) {
            rocket = _value;
    });

    this.__defineGetter__('canvas', function() {
            return canvas;
    });
    this.__defineSetter__('canvas', function(_value) {
            canvas = _value;
    });

    // Chance of spawning an object (in percent per second).
    var spawnChance = 0.8;

    var minSpeed = 20.0;
    var maxSpeed = 50.0;

    var spaceDistance = 10000.0;

    this.__defineGetter__('spaceDistance', function() {
            return spaceDistance;
    });
    this.__defineSetter__('spaceDistance', function(_value) {
            spaceDistance = _value;
    });

    var stageConfig;

    this.__defineGetter__('stageConfig', function() {
            return stageConfig;
    });
    this.__defineSetter__('stageConfig', function(_value) {
            stageConfig = _value;
    });

    this.init = function() {

        // Create ramp sprite.
        var rampSprite = new window.world.Cloud();
        rampSprite.width = 250;
        rampSprite.height = 300;
        rampSprite.type = window.game.faction == 'americans' ? 'rampe_ami' : 'rampe_russ';
        rampSprite.spritePosition.y = rampSprite.height;// rocket.height / 2;
        rampSprite.init();
        backgroundObjects.push(rampSprite);

        colorDiff.r = targetBackgroundColor.r - startBackgroundColor.r;
        colorDiff.g = targetBackgroundColor.r - startBackgroundColor.r;
        colorDiff.b = targetBackgroundColor.r - startBackgroundColor.r;

    }

    this.update = function(dt) {

        // Update background color.
        this.updateBackgroundColor();

        // Update background objects.
        this.updateBackgroundObjects(dt);

        if (rocket.isEngineStarted() &&
            stageConfig != null &&
            stageConfig.backgroundType != null)
        {
            // Check if to create new background object.
            var chance = Math.random();
            var required = spawnChance * dt;
            if (required > chance)
            {
                this.createBackgroundObject();
            }
        }
    }

    this.updateBackgroundColor = function() {
            if (rocket.flyHeight < spaceDistance)
            {
                var ratio = rocket.flyHeight / spaceDistance;
                backgroundColor = {};
                backgroundColor.r = Math.round(startBackgroundColor.r + (colorDiff.r) * ratio);
                backgroundColor.g = Math.round(startBackgroundColor.g + (colorDiff.g) * ratio);
                backgroundColor.b = Math.round(startBackgroundColor.b + (colorDiff.b) * ratio);
                backgroundColor.a = 1.0;
            }
            else
            {
                var transparencyRatio = (rocket.flyHeight - spaceDistance) / spaceDistance;
                backgroundColor.a = 1.0 - transparencyRatio;                
            }
        //debug("bg " + backgroundColor.a);
            canvas.style.backgroundColor = 'rgba(' + backgroundColor.r + ',' + backgroundColor.g + ',' + backgroundColor.b + ',' + backgroundColor.a + ')';
    };

    this.updateBackgroundObjects = function(dt) {
        var i=backgroundObjects.length,
			object;

        while (i--)
        {
            object = backgroundObjects[i];
            object.update(dt);
            
            // Update sprite position.
            var x = Math.round(object.spritePosition.x - object.spriteOffset.x);
            var y = Math.round(object.spritePosition.y - object.spriteOffset.y);

			if (object.type.match('rampe')) {
				y -= rocket.pos.y + 16;
			} else {
				y -= rocket.pos.y - rocket.height / 2;
			}
//			debug(y)
//			if(y > ROCKET.pos.y - 150 && y > ROCKET.pos.y + 400) {
				object.sprite.position(x, window.game.conf.canvas.y - y);
//			} else {
//				object.deinit();
//			}
        }
    }

    this.createBackgroundObject = function() {

        // Get type of background objects.
        var backgroundObjectType = stageConfig.backgroundType;

        // Create background objects.
        var cloud = new backgroundObjectType();
        cloud.spritePosition.x = window.game.conf.canvas.x * Math.random();
        cloud.spritePosition.y = rocket.flyHeight + window.game.conf.canvas.y + cloud.height;
        cloud.speed.x = minSpeed + (maxSpeed - minSpeed) * Math.random();
        cloud.speed.x *= cloud.spritePosition.x < window.game.conf.canvas.x * 0.5 ? 1 : -1;
        cloud.name = "Object " + backgroundObjects.length
        cloud.sprite = window.game.renderEnemy(cloud);
        //cloud.init();
        backgroundObjects.push(cloud);
    }

    this.addBackgroundObject = function(backgroundObject) {
        backgroundObjects.push(backgroundObject);
    }

    this.removeBackgroundObject = function(backgroundObject) {
        var idx = backgroundObjects.indexOf(backgroundObject); // Find the index
        if(idx!=-1) backgroundObjects.splice(idx, 1); // Remove it if really found!
    }
}