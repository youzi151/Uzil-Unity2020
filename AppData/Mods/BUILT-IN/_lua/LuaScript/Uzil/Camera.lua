local Camera = {};

function Camera.inst (key)

	local this = {};

	this.key = key;

	--[[ 取得 位置 ]]
	this.getPosition = function ()
		return Camera.getPosition(this.key);
	end

	--[[ 取得 世界座標 在 鏡頭上的位置 ]]
	this.getWorldToViewPort = function (pos_v3)
		return Camera.getWorldToViewPort(this.key, pos_v3);
	end


	--[[ 取得 視區範圍2D ]] -- array  {u, d, l, r}
	this.getViewBorder2D = function (distance)
		return Camera.getViewBorder2D(this.key, distance);
	end

	return this;
end


--[[ 取得 位置 ]] -- array {x, y}
function Camera.getPosition (key)
	local res = Json.decode(UZAPI.Camera.GetPosition(key));
	res.x = res[1];
	res.y = res[2];
	return res;
end

--[[ 取得 世界座標 在 鏡頭上的位置 ]]
function Camera.getWorldToViewPort (key, pos_v3)
	local x = Util.isOr(pos_v3[1] ~= nil, pos_v3[1], 0);
	local y = Util.isOr(pos_v3[2] ~= nil, pos_v3[2], 0);
	local z = Util.isOr(pos_v3[3] ~= nil, pos_v3[3], 0);
	return Json.decode(UZAPI.Camera.GetWorldToViewPort(key, x, y, z));
end

--[[ 取得 視區範圍2D ]] -- array {u, d, l, r}
function Camera.getViewBorder2D (key, distance)
	if (distance == nil) then distance = 0 end
	local res = Json.decode(UZAPI.Camera.GetViewBorder2D(key, distance));
	res.left = res[1];
	res.right = res[2];
	res.up = res[3];
	res.down = res[4];
	return res;
end

Camera.main = Camera.inst(nil);


return Camera;