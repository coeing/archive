

window.game.enemyManager = function() {

    var rocket;

    var enemies = [];

    var stageConfig;

    var cooldown = 0;

    this.__defineGetter__('rocket', function() {
            return rocket;
    });
    this.__defineSetter__('rocket', function(_value) {
            rocket = _value;
    });

    this.__defineGetter__('stageConfig', function() {
            return stageConfig;
    });
    this.__defineSetter__('stageConfig', function(_value) {
            stageConfig = _value;
    });

    var godMode = false;

    this.__defineGetter__('godMode', function() {
            return godMode;
    });
    this.__defineSetter__('godMode', function(_value) {
            godMode = _value;
    });

    this.init = function() {

//        // Create enemy.
//        var enemy = new stageConfig.objectType();
//        enemy.pos.x = window.game.conf.canvas.x / 2;
//        enemy.pos.y = 300;
//        enemy.speed.x = 0;
//        enemy.speed.y = 0;
//        enemy.init();
//        enemies.push(enemy);
    }

    this.update = function(dt) {

        // Update enemies.
        updateEnemies(dt);

        // Check for collisions.
		try {
			checkCollisions();
		} catch(e) {
			ROCKET.destroy();
		}

        if (rocket.isEngineStarted() &&
            stageConfig != null &&
            stageConfig.objectType != null)
        {
            // Check if new enemy should be created.
            cooldown -= dt;
            if (cooldown <= 0.0)
            {
                createNewEnemy();

                // Setup new cooldown.
                cooldown = stageConfig.minSpawnCooldown + (stageConfig.maxSpawnCooldown - stageConfig.minSpawnCooldown) * Math.random();
            }
        }
    }

    function createNewEnemy() {

        // Get type of new object.
        var objectType = stageConfig.objectType;

        // Determine start position.
        var horizontalRatio = Math.random() * (1.0 + stageConfig.screenSideRatio * 2) - stageConfig.screenSideRatio;
        var minVerticalRatio = stageConfig.minScreenPositionRatio;
        if (horizontalRatio >= 0.0 && horizontalRatio < 1.0)
        {
            minVerticalRatio = 1.0;
        }

        var verticalRatioRange = (stageConfig.maxScreenPositionRatio - minVerticalRatio);
        var verticalRatio = minVerticalRatio + verticalRatioRange * Math.random();

        var startPosition = {'x': horizontalRatio * window.game.conf.canvas.x, 'y': rocket.pos.y + verticalRatio * window.game.conf.canvas.y};

        // Compute instantiation values.
        var speed = {'x': 0, 'y': 0};
        speed.x = stageConfig.minSpeed.x + (stageConfig.maxSpeed.x - stageConfig.minSpeed.x) * Math.random();
        speed.y = stageConfig.minSpeed.y + (stageConfig.maxSpeed.y - stageConfig.minSpeed.y) * Math.random();

        // Switch direction.
        if (horizontalRatio > 0.5)
        {
            speed.x *= -1;
        }
        if (Math.random() < ((verticalRatio - minVerticalRatio) / verticalRatioRange))
        {
            speed.y *= -1;
        }

//        debug("New enemy spawned at " + vectorToString(startPosition) + " with speed " + vectorToString(speed));

        // Create background objects.
        var enemy = new objectType();
        enemy.pos = startPosition;
        enemy.speed = speed;
        enemy.currentAnim = speed.x < 0 ? 0 : 1;
        enemy.name = "Object " + enemy.length
        enemy.init();

        enemies.push(enemy);
    }


    function updateEnemies(dt) {
        var i=enemies.length;
        while (i--)
        {
            var enemy = enemies[i];
            enemy.update(dt);

            // Update sprite position.
            var y = enemy.spritePosition.y;
            y -= rocket.pos.y - rocket.height / 2;
            enemy.sprite.position(enemy.spritePosition.x, window.game.conf.canvas.y - y);
        }
    }

    function checkCollisions() {
		if (rocket.life_points <=0) {
			return;
		}
        // Check each enemy for collision with rocket.
        var i=enemies.length;
        while (i--)
        {
            var enemy = enemies[i];
            if (rocket.collider.checkCollision(enemy.collider)) {
				enemy.explode();
                
                if (!godMode)
                {
                    rocket.life_points -= enemy.damage;
                }

                // Remove enemy.
                enemy.deinit();
                enemies.remove(i);

				// Rocket life points exhausted?
				if (rocket.life_points < 1) {
					throw 'RocketDestroyed';
				}
            }
        }
    }
}