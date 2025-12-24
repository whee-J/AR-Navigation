using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

public static class KakaoRouteParser
{
    public static List<Vector2> Parse(string json)
    {
        List<Vector2> points = new();

        JObject root = JObject.Parse(json);

        var roads = root["routes"][0]["sections"][0]["roads"];

        foreach (var road in roads)
        {
            var vertexes = road["vertexes"];

            for (int i = 0; i < vertexes.Count(); i += 2)
            {
                float lng = vertexes[i].Value<float>();
                float lat = vertexes[i + 1].Value<float>();

                points.Add(new Vector2(lat, lng));
            }
        }

        return points;
    }
}
