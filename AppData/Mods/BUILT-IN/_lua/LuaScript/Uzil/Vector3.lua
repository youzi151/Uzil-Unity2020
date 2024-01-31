local this = {};

function this.toVec2 (v3)
	return {v3[1], v3[2]};
end

--[[ 加 ]]
function this.add (a, b)
	return {a[1]+b[1], a[2]+b[2], a[3]+b[3]};
end

--[[ 減 ]]
function this.sub (a, b)
	return {a[1]-b[1], a[2]-b[2], a[3]-b[3]};
end

--[[ 乘 ]]
function this.mul (a, b)
	if (type(b) == "number") then b = {b, b, b} end
	return {a[1]*b[1], a[2]*b[2], a[3]*b[3]};
end

--[[ 除 ]]
function this.div (a, b)
	if (type(b) == "number") then b = {b, b, b} end
	return {a[1]/b[1], a[2]/b[2], a[3]/b[3]};
end

--[[ 內積 ]]
function this.dot (a, b)
	return (a[1]*b[1]) + (a[2]*b[2]) + (a[3]*b[3]);
end

--[[ 內插 ]]
function this.lerp (a, b, t)
	t = Math.clamp01(t);
	return {
		a[1] + ((b[1] - a[1]) * t),
		a[2] + ((b[2] - a[2]) * t),
		a[3] + ((b[3] - a[3]) * t)
	};
end


--[[ 量 ]]
function this.magnitude (v3)
	return math.sqrt((v3[1] * v3[1]) + (v3[2] * v3[2]) + (v3[3] * v3[3]));
end

--[[ 標準化 ]]
function this.normalize (v3)
	local mag = this.magnitude(v3);
	if (mag == 0) then return v3 end
	return this.div(v3, mag);
end

return this;