using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LinqExtensions
{
    public static T Random<T>(this IEnumerable<T> source)
    {
        T[] arrayForm = source.ToArray();
        return arrayForm[UnityEngine.Random.Range(0, arrayForm.Length)];
    }
}