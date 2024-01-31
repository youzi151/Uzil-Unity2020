
local this = {};

function this.sign (a)
	if (a >= 0) then return 1 else return -1 end
end

function this.clamp (v, min, max)
	if v < min then return min end
	if v > max then return max end
	return v;
end

function this.clamp01 (v)
	return this.clamp(v, 0, 1);
end

function this.lerp (a, b, t)
	return a + ((b - a) * t);
end

function this.lerpAngle (a, b, t)
	local delta = this.repeatNum((b - a), 360);
	if (delta > 180) then delta = delta - 360 end
	return a + delta * this.clamp01(t);
end

function this.repeatNum (t, length)
	return ((t % length) + length) % length;
end

function this.moveTowardsAngle (a, b, maxDelta)
	local deltaAngle = this.deltaAngle(a, b);
	if (-maxDelta < deltaAngle and deltaAngle < maxDelta) then return b end
	b = a + deltaAngle;
	return this.movetowards(a, b, maxDelta);
end

function this.movetowards (a, b, maxDelta)
	if (math.abs(b - a) <= maxDelta) then return b end
	return a + this.sign(b - a) * maxDelta;
end

function this.deltaAngle (a, b)
	local delta = this.repeatNum((b - a), 360);
	if (delta > 180) then delta = delta - 360 end
	return delta;
end


return this;