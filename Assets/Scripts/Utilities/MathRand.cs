using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Provides functions to choose an item from a set of items randomly
/// </summary>
public static class MathRand {
    
    /// <summary>
    /// Pick an item from a given array
    /// </summary>
    /// <typeparam name="T">Type of item array, which is also the type of the item returned</typeparam>
    /// <param name="arr">Array to pick from</param>
    /// <returns></returns>
    public static T Pick<T>(T[] arr)
    {
        return arr[Random.Range(0, arr.Length)];
    }
    /// <summary>
    /// Pick an item from a given List
    /// </summary>
    /// <typeparam name="T">Type of item array, which is also the type of the item returned</typeparam>
    /// <param name="arr">List to pick from</param>
    /// <returns></returns>
    public static T Pick<T>(IList<T> arr)
    {
        return arr[Random.Range(0, arr.Count)];
    }
    /// <summary>
    /// Pick an array index with given probability based on its weight for each index in the array
    /// </summary>
    /// <param name="probs">Array of weights. Doesn't need to add up to 1</param>
    /// <returns></returns>
    public static int WeightedPick(float[] probs)
    {
        float total = 0;

        foreach (float elem in probs)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }
        return probs.Length - 1;
    }
    /// <summary>
    /// Pick an array index with given probability based on its weight for each index in the array
    /// </summary>
    /// <param name="probs">List of weights. Doesn't need to add up to 1</param>
    /// <returns></returns>
    public static int WeightedPick(IList<float> probs)
    {
        float total = 0;

        foreach (float elem in probs)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Count; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }
        return probs.Count - 1;
    }
    /// <summary>
    /// Pick a point on a curve randomly
    /// </summary>
    /// <param name="curve">The curve to pick from</param>
    /// <returns></returns>
    public static float CurveWeightedPick(AnimationCurve curve)
    {
        return curve.Evaluate(Random.value);
    }

    public static void Shuffle<T>(ref T[] deck)
    {
        for (int i = 0; i < deck.Length; i++)
        {
            T temp = deck[i];
            int randomIndex = Random.Range(0, deck.Length);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public static T[] ChooseSetDependent<T>(T[] set, int n)
    {
        T[] result = new T[n];

        int numToChoose = n;

        for (int numLeft = set.Length; numLeft > 0; numLeft--)
        {

            float prob = (float)numToChoose / (float)numLeft;

            if (Random.value <= prob)
            {
                numToChoose--;
                result[numToChoose] = set[numLeft - 1];

                if (numToChoose == 0)
                {
                    break;
                }
            }
        }
        return result;
    }
    /// <summary>
    /// Pick an item from an array of items which has a probability given based on its weight in the probability array
    /// </summary>
    /// <param name="probs">Array of weights. Doesn't need to add up to 1</param>
    /// <param name="items">Array of items to choose from</param>
    /// <returns></returns>
    public static T WeightedPick<T>(float[] probs, T[] items)
    {

        float total = 0;

        foreach (float elem in probs)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
            {
                return items[i];
            }
            else
            {
                randomPoint -= probs[i];
            }
        }
        return items[items.Length - 1];
    }
    /// <summary>
    /// Pick an item from a list of items which has a probability given based on its weight in the probability array
    /// </summary>
    /// <param name="probs">Array of weights. Doesn't need to add up to 1</param>
    /// <param name="items">List of items to choose from</param>
    /// <returns></returns>
    public static T WeightedPick<T>(float[] probs, IList<T> items)
    {

        float total = 0;

        foreach (float elem in probs)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
            {
                return items[i];
            }
            else
            {
                randomPoint -= probs[i];
            }
        }
        return items[items.Count - 1];
    }
}

public static class MathExt
{
    /// <summary>
    /// Check if the integer is between n1 and n2
    /// </summary>
    /// <param name="a"></param>
    /// <param name="n1">Lower limit</param>
    /// <param name="n2">Upper limit</param>
    /// <returns></returns>
    public static bool Between(this int a, int n1, int n2)
    {
        return (a >= n1 && a <= n2);
    }

    /// <summary>
    /// Check if the float is between n1 and n2
    /// </summary>
    /// <param name="a"></param>
    /// <param name="n1">Lower limit</param>
    /// <param name="n2">Upper limit</param>
    /// <returns></returns>
    public static bool Between(this float a, float n1, float n2)
    {
        return (a >= n1 && a <= n2);
    }
}