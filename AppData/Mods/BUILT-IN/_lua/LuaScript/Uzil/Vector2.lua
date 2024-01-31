local this = {};

function this.toVec3 (v2, z)
	if (z == nil) then z = 0; end
	return {v2[1], v2[2], z};
end

--[[ 加 ]]
function this.add (a, b)
	return {a[1]+b[1], a[2]+b[2]};
end

--[[ 減 ]]
function this.sub (a, b)
	return {a[1]-b[1], a[2]-b[2]};
end

--[[ 乘 ]]
function this.mul (a, b)
	if (type(b) == "number") then b = {b, b} end
	return {a[1]*b[1], a[2]*b[2]};
end

--[[ 除 ]]
function this.div (a, b)
	if (type(b) == "number") then b = {b, b} end
	return {a[1]/b[1], a[2]/b[2]};
end

--[[ 內積 ]]
function this.dot (a, b)
	return (a[1]*b[1]) + (a[2]*b[2]);
end

--[[ 內插 ]]
function this.lerp (a, b, t)
	t = Math.clamp01(t);
	return {a[1] + ((b[1] - a[1]) * t), a[2] + ((b[2] - a[2]) * t)};
end

--[[ 旋轉 ]]
function this.rotate (v2, degree)

	local rad = math.rad(-degree);
	local cos = math.cos(rad);
	local sin = math.sin(rad);

	local x = v2[1];
	local y = v2[2];

	return {
		(x * cos) - (y * sin),
		(x * sin) + (y * cos)
	};
end


--[[ 量 ]]
function this.magnitude (v2)
	return math.sqrt((v2[1] * v2[1]) + (v2[2] * v2[2]));
end

--[[ 標準化 ]]
function this.normalize (v2)
	local mag = this.magnitude(v2);
	if (mag == 0) then return v2 end
	return this.div(v2, mag);
end

--[[ 取得角度 ]]
function this.signedAngle (a, b)
	local na = this.normalize(a)
	local nb = this.normalize(b);
	local res = math.acos( Math.clamp(this.dot( na, nb ), -1, 1) ) * 57.29578 * Util.isOr((na[1]*nb[2]) - (na[2]*nb[1]) > 0, 1, -1);
	local greater0 = res > 0;
	res = math.floor(math.abs(res) * 1000) / 1000;
	if (greater0) then return res else return res * -1 end
end

--[[ 取得角度 ]]
function this.angle (a, b)
	return math.acos( Math.clamp(this.dot( this.normalize(a), this.normalize(b) ), -1, 1) ) * 57.29578;
end

return this;