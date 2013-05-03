/**
 * Helper for basic inheritance
 * @param F Function
 */
Function.prototype.inherits = function(F) {
	if (!F) {
		throw new TypeError();
	}
	this.prototype = new F;
	this.prototype.constructor = this;
	this.prototype.parent = F.prototype;
};

Number.prototype.bound = function(min, max) {
	if (min > max) {
		[min, max] = [max, min];
	}
	return Math.min(Math.max(this, min), max);
};

Number.prototype.between = function(a, b) {
	if (a === undefined || b === undefined) {
		throw new TypeError('Invalid Arguments.');
	}
	return (a < b ? this >= a && this <= b : this.between(b, a));
};

// Some array functions - By John Resig (MIT Licensed)
Array.prototype.remove = function(from, to) {
  var rest = this.slice((to || from) + 1 || this.length);
  this.length = from < 0 ? this.length + from : from;
  return this.push.apply(this, rest);
};

Array.prototype.max = function(array){
	return Math.max.apply(Math, array);
};

Array.prototype.min = function(array) {
	return Math.min.apply(Math, array );
};

/**
 * splits a string into parts of given length.
 *
 * @param {Number} len
 * @return {Array}
 */
String.prototype.split2 = function(len) {
	var array = [],
		index = 0,
		length = this.length;

	if (len > length)
		throw new Error('Argument len is longer than the actual length of this string.');

	for (; index < length; index += len) {
		array.push(this.substr(index, len));
	}

	return array;
};

String.prototype.removeClass = function(str) {
	var classes = this.split(/\W+/), i, index;

	str = str.split(/\W+/);

	i = str.length;

	while (i--) {
		if ((index = classes.indexOf(str[i])) >= 0) {
			classes.remove(index);
		}
	}
	return classes.join(' ');
};

/**
 * Removes a class.
 *
 * @param str String
 * @return HTMLElement
 */
HTMLElement.prototype.removeClass = function(str) {
	if (!str) {
		return this;
	}
	var classes = this.className.split(/[^\w-]+/), i, index;
	str = str.split(/[^\w-]+/);

	i = str.length;

	while (i--) {
		if ((index = classes.indexOf(str[i])) !== -1) {
			classes.remove(index);
		}
	}

	this.className = classes.join(' ');
	return this;
};

HTMLElement.prototype.addClass = function(str) {
	if (!str) {
		return this;
	}
	var classes = this.className.split(/[^\w-]+/), i, index;
	str = str.split(/[^\w-]+/);

	i = str.length;

	while (i--) {
		if (classes.indexOf(str[i]) !== -1) {
			continue;
		}
		classes.push(str[i]);
	}

	this.className = classes.join(' ');
	return this;
};

/**
 * @deprecated bullshit. do not use.
 */
HTMLElement.prototype.delegate = function(types, callback) {
	return this.bind(types, callback, this);
};

HTMLElement.prototype.append = function(child) {
	this.appendChild(child);
	return this;
};

/**
 * Binds a callback function on an eLement.
 *
 * @example
 * toolkit.get('id').bind({
 *     'click.namespace || keyup': callback
 * });
 *
 * @param types String
 * @param callback Function
 *
 * @return HTMLElement
 */
HTMLElement.prototype.bind = function(types, callback, target) {
	var that = this, namespace;

	if (typeof types === typeof {}) {
		types.each(function(callback, type) {
			that.bind(type, callback, target);
		});
		return this;
	}

	types = types.split(/\W+\|\|\W+/);

	types = types.map(function(str) {
		return str.split('.');
	});

	types.each(function(type) {

		if (type.length > 1) {
			callback.namespace = (namespace = type[1]);
		}

		toolkit.addEventListener(type[0], target);

		if (!this.callback) {
			this.callback = {};
			this.callback[type[0]] = [callback];
		} else {
			if (this.callback[type[0]]) {
				this.callback[type[0]].push(callback);
			} else {
				this.callback[type[0]] = [callback];
			}
		}
	// use document for events that are supposed to bubble up
	}, target || this);

	return this;
};

/**
 * bind a function that's removed after the first execution.
 *
 * @example
 * toolkit.get('id').bind({
 *     'click.namespace || keyup': callback
 * });
 *
 * @param types String or Object
 * @param callback Function
 */
HTMLElement.prototype.once = function(types, callback) {
	var that = this, unique;


	if (typeof types === typeof {}) {
		types.each(function(callback, type) {
			that.once(type, callback);
		});
		return this;
	}

	types = types.split(/\W+\|\|\W+/);

	types = types.map(function(str) {
		return str.split('.');
	});

	unique = types[0] + '.' + Date.now().toString();

	this.bind(unique, function() {
		callback.call(that);
		that.unbind(unique);
	});

	return this;
};

/**
 * TODO: namespacing
 * @param e Event
 */
HTMLElement.prototype.trigger = function(e) {
	var arr, i, type;

	//TODO: real events
	if (typeof e === 'string') {
		type = [e];
	} else {
		type = e.type.split('.');
	}

	if (!this.callback || !(arr = this.callback[type[0]])) {
		return;
	}
	i = arr.length;
	// namespacing without global cache? will be slooooow
	if (!type[0] && type[1]) {
		throw 'Not implemented yet.';
	}

	while (i--) {
		if (typeof arr[i] === 'function') {
			arr[i].call(this, e);
		}
	}
};

/**
 * TODO: namespacing
 * @param param String || Function
 */
HTMLElement.prototype.unbind = function(param) {
	var type = (typeof param).toLowerCase(),
		i, tmp, namespace;

	switch (type) {
		case 'string':
			param = param.split('.');
			type  = param[0];
			namespace = param[1];
			// remove all
			if (type && namespace) {
				tmp = this.callback[type];
				i   = tmp.length;
				while (i--) {
					if (tmp[i].namespace === namespace) {
						tmp.remove(i);
					}
				}

			} else if (type) {
				throw 'Not implemented yet.';
			} else if (namespace) {
				throw 'Not implemented yet.';
			}
			break;
		case 'function':
			throw 'Not implemented yet.';
			break;
	}
};

/**
 * Turns a string into DOM elements
 */
String.prototype.toDOMElement = function() {
	var div = document.createElement('div');
	div.innerHTML = this;

	return div.childNodes;
};

window.toolkit = new function() {

	var listenerList = {};


	this.get = function get(id) {
		return document.getElementById(id);
	};

	this.append = function(stuff) {
		var type = typeof stuff;

		switch (type) {
			case 'string':
				stuff.toDOMElement();
				break;
		}

	}

	this.stringify = function stringify(obj) {
		var key, str = '';
		label:
		for (key in obj) {
			if(!obj.hasOwnProperty(key)){
				break label;
			}

			if (typeof obj[key] === 'object') {
				str += stringify(obj[key]);
			} else {
				str += key + ':' + obj[key] + ';\n';
			}
		}

		return str;
	};

	/**
	 * @return HTMLElement
	 */
	this.create = function create(obj) {
		var i, elem, ref, tag = obj.tag || 'div', fragment,
			attributify = function(value, key) {
				var attr = value;

				if (typeof attr === 'object') {
					ref = ref[key];
					attr.each(attributify);
				} else {
					ref[key] = attr;
				}
			};
		delete obj.tag;

		elem = document.createElement(tag);

		if (typeof obj === typeof '') {
			fragment = document.createDocumentFragment();
			elem.innerHTML = obj;

			while (elem.childNodes.length) {
				fragment.appendChild(elem.childNodes[0]);
			}

			elem = null;
			return fragment;
		} else {
			ref = elem;
			// apply attributes
			// TODO: maybe cssdeclaration would be more elegant:
			obj.each(attributify);
		}
		return elem;
	};

	/**
	 * checks if the event listener for a certain type is attached to the document
	 *
	 * @param {String} type
	 * @return {Boolean}
	 */
	function isEventListenerAdded(type) {

		return listenerList.hasOwnProperty(type);
	}

	/**
	 *
	 * @param {String} type
	 * @param {HTMLElement} target (optional)
	 *
	 * @return self
	 */
	this.addEventListener = function(type, target) {
		if (isEventListenerAdded(type)) {
			return;
		}

		var useCapture = ['focus', 'transitionend','TODO'].indexOf(type) !== -1;
		listenerList[type] = function(e) {
			var _target = target || (e.target || {});

			if (typeof _target.trigger === 'function') {
				_target.trigger(e);
			}
		}
		var target2 = target || document

		target2.addEventListener(type, listenerList[type], useCapture);

	};

	return this;
}();

window.toolkit.each = function(callback, context) {
	var i;
	if (this instanceof NodeList) {
		console.log('This is a NodeList. Be careful.');
	}

	if (context) {
		if (this instanceof Array || this instanceof NodeList) {
			i = this.length;
			while (i--) {
				callback.call(context, this[i], i);
			}
		} else {
			for (i in this) {
				if (this.hasOwnProperty(i)) {
					callback.call(context, this[i], i);
				}
			}
		}
	} else {
		if (this instanceof Array || this instanceof NodeList) {
			i = this.length;
			while (i--) {
				callback(this[i], i);
			}
		} else {
			for (i in this) {
				if (this.hasOwnProperty(i)) {
					callback(this[i], i);
				}
			}
		}
	}

};

/**
 * debug function.
 * @param sth {Object}
 *
 */
window.debug = function(sth) {
	if (arguments.length <= 1) {
		return console.log(sth);
	}
	console.group('toolkit.debug:');
	if (typeof arguments.each != 'function') {
		arguments.each = window.toolkit.each
	}
	arguments.each(function(a) {
		console.log(a);
	});
	return console.groupEnd();

};

window.requestAnimFrame = (function(){
	return window.requestAnimationFrame       ||
		window.webkitRequestAnimationFrame ||
		window.mozRequestAnimationFrame    ||
		window.oRequestAnimationFrame      ||
		window.msRequestAnimationFrame;
})();