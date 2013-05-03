window.collision.Shape = function Shape() {
    var offset = {
			'x': 0,
			'y': 0
		};

    this.__defineGetter__('offset', function() {
            return offset;
    });
        
    this.__defineSetter__('offset', function(_value) {
            offset = _value;
    });
                
    this.collides = function collides(ownTransform, other, otherTransform) {
        return false;
    }
    
    this.collideCircleCircle = function collideCircleCircle(transformA, radiusA, transformB, radiusB) {
        // Compute distance.
        var distanceX = Math.abs(transformA.x - transformB.x);
        var distanceY = Math.abs(transformA.y - transformB.y);
        var squareDistance = distanceX * distanceX + distanceY * distanceY;
        
        return squareDistance <= (radiusA + radiusB) * (radiusA + radiusB);
    }
    
    this.collideCircleRect = function collideCircleRect(circleCenter, circleRadius, rectCenter, rectWidth, rectHeight) {
                
        // Compute circle distance.
        var circleDistance = { 'x':0, 'y':0 };
        circleDistance.x = Math.abs(circleCenter.x - rectCenter.x);
        circleDistance.y = Math.abs(circleCenter.y - rectCenter.y);

        if (circleDistance.x > (rectWidth/2 + circleRadius)) { return false; }
        if (circleDistance.y > (rectHeight/2 + circleRadius)) { return false; }

        if (circleDistance.x <= (rectWidth/2)) { return true; } 
        if (circleDistance.y <= (rectHeight/2)) { return true; }

        var cornerDistance_sq = (circleDistance.x - rectWidth/2)^2 +
                            (circleDistance.y - rectHeight/2)^2;

        return (cornerDistance_sq <= (circleRadius * circleRadius));
    }
    
    this.collideRectRect = function collideRectRect(rectCenterA, widthA, heightA, rectCenterB, widthB, heightB) {
        var minXA = rectCenterA.x - widthA/2;
        var maxXA = rectCenterA.x + widthA/2;
        var minXB = rectCenterB.x - widthB/2;
        var maxXB = rectCenterB.x + widthB/2;
        if (minXA < minXB && maxXA < minXB) { return false; }
        if (minXA > maxXB && maxXA > maxXB) { return false; }
        
        var minYA = rectCenterA.y - heightA/2;
        var maxYA = rectCenterA.y + heightA/2;
        var minYB = rectCenterB.y - heightB/2;
        var maxYB = rectCenterB.y + heightB/2;
        if (minYA < minYB && maxYA < minYB) { return false; }
        if (minYA > maxYB && maxYA > maxYB) { return false; }
        
        return true;
    }
}

window.collision.CircleShape = function CircleShape(_radius) {  
    
    window.collision.Shape.call(this);
        
    var radius = _radius;  
                
    this.collides = function collides(ownTransform, other, otherTransform) {
        if (other.type == "Rect")
        {
            return this.collideCircleRect(ownTransform, this.radius, otherTransform, other.width, other.height);
        }
        else if (other.type == "Circle")
        {
            return this.collideCircleCircle(ownTransform, this.radius, otherTransform, other.radius);          
        }
        return false;
    }  
    
    this.__defineGetter__('radius', function() {
            return radius;
    });
}

window.collision.CircleShape.inherits(window.collision.Shape);
window.collision.CircleShape.prototype.type = "Circle";

window.collision.RectShape = function RectShape(_width, _height) {
    
    window.collision.Shape.call(this);
    
    var width = _width;
    var height = _height;
                
    this.collides = function collides(ownTransform, other, otherTransform) {
        if (other.type == "Rect")
        {
            return this.collideRectRect(ownTransform, this.width, this.height, otherTransform, other.width, other.height);
        }
        else if (other.type == "Circle")
        {
            return this.collideCircleRect(otherTransform, other.radius, ownTransform, this.width, this.height);          
        }
        return false;
    }
    
    this.__defineGetter__('width', function() {
            return width;
    });
    
    this.__defineGetter__('height', function() {
            return height;
    });
}

window.collision.RectShape.inherits(window.collision.Shape);
window.collision.RectShape.prototype.type = "Rect";