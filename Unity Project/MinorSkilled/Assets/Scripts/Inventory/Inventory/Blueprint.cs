using System;
using System.Collections.Generic;

[Serializable]
public class Blueprint
{
    public Item finalItem;
    public List<Ingredient> ingredients;

    public Blueprint()
    {
        finalItem = new Item();
        ingredients = new List<Ingredient>();
    }

    public Blueprint(Item firstItem, Item secondItem, Item thirdItem)
    {
        finalItem = new Item();
        ingredients = new List<Ingredient>();
        ingredients[0].item = firstItem;
        ingredients[1].item = secondItem;
        ingredients[2].item = thirdItem;
    }

    public override string ToString()
    {
        string output = "\n";
        for (int i = 0; i < ingredients.Count; i++)
        {
            Ingredient ingredient = ingredients[i];
            output += "Ingredient "+i+": "+ingredient.item.ID+"\t";
        }
        return finalItem + output;
    }
}
