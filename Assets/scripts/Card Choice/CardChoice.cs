using System;
using UnityEngine;
using UnityEngine.UI;

public class CardChoice : MonoBehaviour
{
   public Animator animatorCards1, animatorCards2;

   public ItemSO itemPool;
   
   public PlacementSystem placementPlayer1, placementPlayer2;

   [Serializable]
   public struct CardsPNG
   {
      public Image face, dos;
   }
   public CardsPNG[] cardsUI1, cardsUI2;

   public bool player1Fail, player2Fail;

   public int[] cardsID1, cardsID2;


   public void Launchmamere()
   {
      ResolveMiniGameResults(2, 2);
   }
   
   public void ResolveMiniGameResults(int miniGame1, int miniGame2)
   {
      
      ResolvePlayer(miniGame1, ref player1Fail, ref cardsID1, cardsUI1);
      ResolvePlayer(miniGame2, ref player2Fail, ref cardsID2, cardsUI2);
      
      if(!player1Fail) animatorCards1.SetTrigger("Start");
      if(!player2Fail) animatorCards2.SetTrigger("Start");
   }
   void ResolvePlayer(int result, ref bool playerFail, ref int[] cardsID, CardsPNG[] cardsUI)
   {
      if (result <= 0) { playerFail = true; return; }
      playerFail = false;

      // Définition des raretés à tirer
      int[] raritiesToPick = result switch
      {
         1 => new[] { 0, 0, 0 }, 
         2 => new[] { 0, 0, 1 }, 
         3 => new[] { 0, 1, 2 }, 
         _ => new[] { 0, 0, 0 }
      };

      for (int i = 0; i < 3; i++)
      {
         int rarity = raritiesToPick[i];
         
         var possibleItems = itemPool.itemsData.FindAll(item => item.rarity == rarity);

         if (possibleItems.Count == 0) continue;
         
         ItemsData chosenItem = possibleItems[UnityEngine.Random.Range(0, possibleItems.Count)];

         cardsID[i] = chosenItem.Id;
         cardsUI[i].face.sprite = chosenItem.carte;
         cardsUI[i].dos.sprite = chosenItem.dosCarte;
      }
   }


   public void ChoseCard(int ID)
   {
      bool isPlayer1 = ID <= 2;
      if (isPlayer1)
      {
         
         if(ID ==0) animatorCards1.SetTrigger("1");
         if(ID ==1) animatorCards1.SetTrigger("2");
         if(ID ==2) animatorCards1.SetTrigger("3");
         
         placementPlayer1.StartPlacement(cardsID1[ID]);
      }
      else
      {
         
         if(ID ==3) animatorCards2.SetTrigger("1");
         if(ID ==4) animatorCards2.SetTrigger("2");
         if(ID ==5) animatorCards2.SetTrigger("3");
         
         placementPlayer2.StartPlacement(cardsID2[ID-3]);
      }
   }
}
