window.enemies = {}

window.enemies.Enemy = function Enemy() {
    window.world.SpriteObject.call(this);

    var collider = new window.collision.Collider(),
		damage = 10; // default damage to deal when hit
        
    var explosionType = window.world.SmallExplosion;
	this.__defineGetter__('explosionType', function() {
		return explosionType;
	});
	this.__defineSetter__('explosionType', function(_value) {
		explosionType = _value;
	});   
    
    var explosion;

    this.__defineGetter__('collider', function() {
            return collider;
    });

    this.__defineGetter__('pos', function() {
            return collider.pos;
    });
    this.__defineSetter__('pos', function(_value) {
            collider.pos = _value;
        
            // Update sprite position.
            this.spritePosition.x = Math.round(collider.pos.x - this.spriteOffset.x);
            this.spritePosition.y = Math.round(collider.pos.y - this.spriteOffset.y);
    });

	this.__defineGetter__('damage', function() {
		return damage;
	});
	this.__defineSetter__('damage', function(_damage) {
		damage = _damage;
	});   
    
    this.update = function update(dt) {
        // Update position.
        collider.pos.x += this.speed.x * dt;
        collider.pos.y += this.speed.y * dt; 
        
            // Update sprite position.
            this.spritePosition.x = Math.round(collider.pos.x - this.spriteOffset.x);
            this.spritePosition.y = Math.round(collider.pos.y - this.spriteOffset.y);
    };

	this.explode = function explode() {
		explosion = new explosionType();        
		explosion.spritePosition.x = this.pos.x - explosion.width / 2;        
		explosion.spritePosition.y = this.pos.y + explosion.height / 2;
		explosion.spritePosition.z = 30.0;
		explosion.init();
        
        // Let background manager take care of position updates.
        window.game.engine.backgroundManager.addBackgroundObject(explosion);
                                
		explosion.sprite.callback(this.onExplosionIsOver, 1);
	};

	this.onExplosionIsOver = function onExplosionIsOver() {
        
        explosion.sprite.position(1000, 1000);
        explosion.sprite.remove();
        
        // Remove from background manager.
        window.game.engine.backgroundManager.removeBackgroundObject(explosion);
	}
}

window.enemies.Enemy.inherits(window.world.SpriteObject);

window.enemies.Bird = function Bird() {   
    
    window.enemies.Enemy.call(this);    
    
	this.damage = 12.5;
	this.width  = 19.5;
	this.height = 17;
        this.spriteOffset.x = 19.5;
        this.spriteOffset.y = -17;
	this.frames = 1;
	this.anim   = 2;
        this.animSpeed  = 30;
        var shape = new window.collision.CircleShape(17);
        this.collider.shapes.push(shape);
    this.explosionType = window.world.BirdExplosion;
}

window.enemies.Bird.inherits(window.enemies.Enemy);
window.enemies.Bird.prototype.type = 'sprite_birds';

window.enemies.Balloon = function Balloon() {   
    
    window.enemies.Enemy.call(this);    
    
	this.damage = 25;
    this.width  = 54;
    this.height = 76;
    this.spriteOffset.x = 54;
    this.spriteOffset.y = -76;
    this.frames = 1;
    this.anim   = 1;
    this.animSpeed = 0;
    var shapeCircle = new window.collision.CircleShape(52);
    shapeCircle.offset.y = 22;
    this.collider.shapes.push(shapeCircle);
    var shapeRect = new window.collision.RectShape(28, 40);
    shapeRect.offset.y = -56;
    this.collider.shapes.push(shapeRect);
    this.explosionType = window.world.Explosion;
}

window.enemies.Balloon.inherits(window.enemies.Enemy);
window.enemies.Balloon.prototype.type = 'sprite_balloon';

window.enemies.Satellite = function Satellite() {   
    
    window.enemies.Enemy.call(this);    
    
	this.damage = 40;
    this.width  = 102;
    this.height = 24;
    this.spriteOffset.x = 102;
    this.spriteOffset.y = -24;
    this.frames = 1;
    this.anim   = 1;
    this.animSpeed = 0;
    var shapeRect = new window.collision.RectShape(204, 48);
    this.collider.shapes.push(shapeRect);
    this.explosionType = window.world.Explosion;
}

//window.enemies.Satellite.inherits(window.enemies.Enemy);
window.enemies.Satellite.prototype.type = 'sprite_satellite';

window.enemies.Astroid = function Astroid() {   
    
    window.enemies.Enemy.call(this);    
    
	this.damage = 75;
    this.width  = 73;
    this.height = 73;
    this.spriteOffset.x = 73;
    this.spriteOffset.y = -73;
    this.frames = 1;
    this.anim   = 1;
    this.animSpeed = 0;
    var shapeCircle = new window.collision.CircleShape(73);
    this.collider.shapes.push(shapeCircle);
    this.explosionType = window.world.Explosion;
}

window.enemies.Astroid.prototype.type = 'sprite_meteor_big';