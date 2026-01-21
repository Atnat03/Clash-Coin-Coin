using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CardChoice : MonoBehaviour
{
   public static CardChoice instance;
   
   public Animator animatorCards1, animatorCards2;

   ItemSO itemPool1, itemPool2;
   
   public PlacementSystem placementPlayer1, placementPlayer2;

   [Serializable]
   public struct CardsPNG
   {
      public Image face, dos;
   }
   public CardsPNG[] cardsUI1, cardsUI2;


   public int[] cardsID1, cardsID2;
   
   public int cardSelected1;
   public int cardSelected2;

   public bool inSelection1, inSelection2 = false;

   float lastAimX1;
   float lastAimX2;

   private void Awake()
   {
      instance = this;
      
      itemPool1 = VariablesManager.instance.duckItemDatabase;
      itemPool2 = VariablesManager.instance.frogItemDatabase;
   }

   private void OnDisable()
   {
      VariablesManager.instance.players[0].OnClicked -= Player1Click;
      VariablesManager.instance.players[1].OnClicked -= Player2Click;
   }

   private void Player1Click()
   {
      ChoseCard(cardSelected1);
   }
   
   private void Player2Click()
   {
      ChoseCard(cardSelected2+3);
   }
   public void Update()
   {
      if (inSelection1)
      {
         float x = VariablesManager.instance.players[0].aimInput.x;

         if (x > 0.9f && lastAimX1 <= 0.9f)
            cardSelected1++;
         else if (x < -0.9f && lastAimX1 >= -0.9f)
            cardSelected1--;

         cardSelected1 = Mathf.Clamp(cardSelected1, 0, 2);
         UpdateCardSelected();

         lastAimX1 = x;
      }

      if (inSelection2)
      {
         float x = VariablesManager.instance.players[1].aimInput.x;

         if (x > 0.9f && lastAimX2 <= 0.9f)
            cardSelected2++;
         else if (x < -0.9f && lastAimX2 >= -0.9f)
            cardSelected2--;

         cardSelected2 = Mathf.Clamp(cardSelected2, 0, 2);
         UpdateCardSelected();

         lastAimX2 = x;
      }
   }

   public void UpdateCardSelected()
   {
      for (int i = 0; i < 3; i++)
      {
         cardsUI1[i].face.color = Color.white;
         cardsUI2[i].face.color = Color.white;
      }
      
      if(cardSelected1 != -1) cardsUI1[cardSelected1].face.color = new Color(0.7f, 0.7f, 0.7f);
      if(cardSelected2 != -1)cardsUI2[cardSelected2].face.color = new Color(0.7f, 0.7f, 0.7f);
   }
   
   public void ResolveMiniGameResults(int miniGame1, int miniGame2)
   {
      inSelection1 = true;
      inSelection2 = true;
      
      VariablesManager.instance.players[0].OnClicked += Player1Click;
      VariablesManager.instance.players[1].OnClicked += Player2Click;

      cardSelected1 = 0;
      cardSelected2 = 0;
      ResolvePlayer(miniGame1, ref cardsID1, ref itemPool1, cardsUI1);
      ResolvePlayer(miniGame2, ref cardsID2, ref itemPool2, cardsUI2);
      
      animatorCards1.SetTrigger("Start");
      animatorCards2.SetTrigger("Start");
   }
   void ResolvePlayer(int result, ref int[] cardsID, ref ItemSO itemPool, CardsPNG[] cardsUI)
   {
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
         inSelection1 = false;
         placementPlayer1.currentItemToPlace = cardsID1[ID];
      }
      else
      {
         inSelection2 = false;
         if(ID ==3) animatorCards2.SetTrigger("1");
         if(ID ==4) animatorCards2.SetTrigger("2");
         if(ID ==5) animatorCards2.SetTrigger("3");
         placementPlayer2.currentItemToPlace = cardsID2[ID-3];
      }
   }

   public void ResetCardSolves()
   {
      inSelection1 = false;
      inSelection2 = false;
      
      Debug.Log("ResetCardSolves → inSelection1/2 forcé à false");
   }

}
