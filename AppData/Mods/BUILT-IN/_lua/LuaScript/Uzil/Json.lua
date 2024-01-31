local this = {};

local cjson = require "cjson.safe";
local dkjson = require "Lib/dkjson";

function this.encode (tb)
    if (tb == nil) then return nil end

    local str;

    -- 若 已經是字串
    if (type(tb) == "string") then
        -- local starts = string.sub(table, 1, 1);

        -- if (starts == "{" or starts == "[") then
        --     return table;
        -- end

        return tb;
    end

    str = cjson.encode(tb);

    if (str == nil or str == "") then
        str = dkjson.encode(tb);
    end

    if (type(str) ~= "string") then return nil end

    return str;
end

function this.decode (json)
    if (json == nil) then return nil end

    local tb;

    -- 若 已經是Table
    if (type(json) == "table") then return json end

    tb = cjson.decode(json);

    if (tb == nil) then
        tb = dkjson.decode(json);
    end

    if (type(tb) ~= "table") then return nil end

    return tb;
end

return this;