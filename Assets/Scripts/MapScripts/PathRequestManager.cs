using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRquest;

    static PathRequestManager instance;

    private void Awake()
    {
        instance = this;
    }

    public static void RequestPath(Vector2 pathStart, Vector2 pathEnd, Action<Vector2, bool> callback)
    {
        PathRequest newRequest = new PathRequest();
    }

    struct PathRequest
    {
        public Vector2 pathStart;
        public Vector2 pathEnd;
        public Action<Vector2[], bool> callback;

        public PathRequest(Vector2 _start, Vector2 _end, Action<Vector2[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
}
