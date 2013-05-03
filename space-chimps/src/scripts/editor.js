window.RocketEditor = function RocketEditor() {
	var elm = {
			'list': document.getElementById('parts'),
			'money': document.getElementById('money'),
			'canvas': document.querySelector('canvas'),
			'weightbar': document.querySelector('#weight .bar-bar'),
			'fuelbar': document.querySelector('#fuel .bar-bar'),
			'thrustbar': document.querySelector('#thrust .bar-bar')
		},
		mibbu = window.game.engine.mibbu,
		that = this;

    var available_parts = [],
		i;

    // Fragment which contains the available parts.
    var fragment;

    // Initialization.
	function init() {
		// add all rocket parts to list
		for (i in window.rocket) {
			if (window.rocket[i].prototype.type && !window.rocket[i].prototype.is_base) {
				available_parts.push(new window.rocket[i]());
			}
		}

		elm.list.addEventListener('click', onPartClicked);

        that.updateMoneyDisplay();
        that.updateInfo();
	}

    function deinit() {
        elm.list.removeEventListener('click', onPartClicked);

        // TODO(co): Not very clean, find a better way.
        var partElements = elm.list.getElementsByTagName('li');
        var i = partElements.length;
        while (i--)
        {
            elm.list.removeChild(partElements[i]);
        }
    }

	this.addPart = function(part) {

        // Add part.
        if (part.cost > window.game.money) {
			window.game.audio.error.play();
            return false;
        }
        window.game.audio.addpart.play();

        var added = window.ROCKET.addPart(part);
        if (added)
        {
            window.game.money -= part.cost;
            window.game.renderRocket(window.ROCKET);
        }

		return added;
	};


	this.removePart = function(part) {
		window.game.audio.addpart.play();
        window.ROCKET.removePart(part);
		window.game.money += part.cost;
		window.game.renderRocket(window.ROCKET);
	};


	this.updateInfo = function updateInfo() {
		var maxpx = 124,
			maxweight = 5000,
			maxfuel = 50000,
			maxthrust = 50000;

		elm.weightbar.style.width = window.ROCKET.weight / maxweight * maxpx + 'px';
		elm.fuelbar.style.width = window.ROCKET.fuel / maxfuel * maxpx + 'px';
//		elm.fuelbar.style.width = window.ROCKET.fuel / maxfuel * maxpx + 'px';

		var i,
			part,
			li, a, p, span, h4, div;

        fragment = document.createDocumentFragment();



		// draw available parts
		i = available_parts.length;
		while (i--) {
			part = available_parts[i];
			a = document.createElement('a');
			li = document.createElement('li');
			p = document.createElement('p');
			span = document.createElement('span');
			h4 = document.createElement('h4');
			div = document.createElement('div');

			div.style.backgroundImage = 'url(' + part.image +')';
			div.style.width = part.width + 'px';
			div.style.height = (part.type === 'nuclearengine' ? 46 : part.height) + 'px';


			span.innerHTML = part.cost;
			span.className = 'part-cost money-icon' +  ((window.game.money < part.cost) ? ' not-enough' : '');

			a.href = '#' + part.type;

			h4.innerHTML = part.type.toUpperCase();
			h4.className = 'part-type';

			p.innerHTML = part.descr;
			p.className = 'part-descr';

			li.appendChild(div);
			li.appendChild(span);
			li.appendChild(h4);
			li.appendChild(p);
			li.appendChild(a);
			fragment.appendChild(li);
		}

		elm.canvas.style.left = '-101px';

		elm.list.innerHTML = '';
		elm.list.appendChild(fragment);

	};

	this.close = function() {
		var editor = document.getElementById('editor');
//		editor.parentNode.removeChild(editor);
		editor.style.width = '0px';
		elm.canvas.style.left = '';
		window.ROCKET.init();

        // Deinitialize editor.
        deinit();

		return true;
	};

    this.updateMoneyDisplay = function() {
		elm.money.innerHTML = window.game.money;
    }

    function onPartClicked(e) {
        var href = (e.target.href || '').split(/#/).reverse()[0],
            parent = e.target.parentNode;

        if (!href) {
            return;
        }

        e.preventDefault();

        var newpart = available_parts.filter(function(elem) {
            return elem.type == href;
        });

        if (!window.ROCKET.hasPart(newpart[0])) {
            if (that.addPart(newpart[0])) {
                parent.className = 'selected';
            }
        } else
        {
            that.removePart(newpart[0]);
            parent.className = '';
        }

        that.updateMoneyDisplay();

        that.updateInfo();
    }

	init();
};


window.game.renderRocket = function renderRocket(rocket, rocketScreenPosition) {
	var part,
		parts = rocket.parts,
		sprites = rocket.sprites,
		pos = rocket.screenpos,
		height = 0,
		i = parts.length, type, j;


	// clear rocket sprites
	for (j in sprites) {
		sprites[j].remove();
	}

	while (i--) {
		part = parts[i];

		if (!part) {
			return rocket.sprites;
		}

		type = part.type;
		sprites[type] = new window.game.engine.mibbu.spr(part.image, part.width, part.height, part.frames, part.anim);


		sprites[type].position(pos.x, pos.y + height, 2).speed(part.animSpeed)
		height += part.height;
	}

	return rocket.sprites = sprites;
};

window.game.renderEnemy = function renderEnemy(enemy) {
    var sprite = new window.game.engine.mibbu.spr(enemy.image, enemy.width, enemy.height, enemy.frames, enemy.anim);
    sprite.position(enemy.spritePosition.x, enemy.spritePosition.y, enemy.spritePosition.z);
    sprite.speed(enemy.animSpeed);
    sprite.animation(enemy.currentAnim);
    return sprite;
};