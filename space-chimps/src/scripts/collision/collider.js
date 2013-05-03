window.collision = {};

var addVectors = function(vectorA, vectorB) {
    return { 'x': vectorA.x + vectorB.x, 'y': vectorA.y + vectorB.y};
}

function vectorToString(vector) {
    return "(x:" + vector.x + ", y:" + vector.y + ")";
}

window.collision.Collider = function Collider() {
        var shapes = [],
          position = {
			'x': 0,
			'y': 0
		};

        this.checkCollision = function checkCollision(other) {
            // Check collision between shapes of both colliders.
            var i;
            var length = shapes.length;
            var j;
            var lengthOther = other.shapes.length;
            for (i = 0; i < length; i++)
            {
                var shape = shapes[i];
                var shapePosition = addVectors(this.pos, shape.offset);
                for (j = 0; j < lengthOther; j++)
                {
                    var otherShape = other.shapes[j];
                    var otherShapePosition = addVectors(other.pos, otherShape.offset);
                    if (shape.collides(shapePosition, otherShape, otherShapePosition))
                    {
                        return true;
                    }
                }
            }

            return false;
        };
        

        this.__defineGetter__('pos', function() {
                return position;
        });
        
	this.__defineSetter__('pos', function(_value) {
		position = _value;
	});

        this.__defineGetter__('shapes', function() {
                return shapes;
        });
};