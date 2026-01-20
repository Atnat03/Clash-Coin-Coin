using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CardChoice : MonoBehaviour
{
   public static CardChoice instance;
   
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
   
   public int cardSelected1;
   public int cardSelected2;

   public bool inSelection1, inSelection2 = false;

   float lastAimX1;
   float lastAimX2;

   private void Awake()
   {
      instance = this;
   }

   private void OnDisable()
   {
      SpawnPlayer.instance.players[0].OnClicked -= Player1Click;
      SpawnPlayer.instance.players[1].OnClicked -= Player2Click;
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
         float x = SpawnPlayer.instance.players[0].aimInput.x;

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
         float x = SpawnPlayer.instance.players[1].aimInput.x;

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
      
      if(cardSelected1 != -1) cardsUI1[cardSelected1].face.color = new Color(0.3f, 0.3f, 0.3f);
      if(cardSelected2 != -1)cardsUI2[cardSelected2].face.color = new Color(0.3f, 0.3f, 0.3f);
   }
   
   public void ResolveMiniGameResults(int miniGame1, int miniGame2)
   {
      inSelection1 = true;
      inSelection2 = true;
      
      SpawnPlayer.instance.players[0].OnClicked += Player1Click;
      SpawnPlayer.instance.players[1].OnClicked += Player2Click;

      cardSelected1 = 0;
      cardSelected2 = 0;
      ResolvePlayer(miniGame1, ref player1Fail, ref cardsID1, cardsUI1);
      ResolvePlayer(miniGame2, ref player2Fail, ref cardsID2, cardsUI2);
      
      if(!player1Fail) animatorCards1.SetTrigger("Start");
      if(!player2Fail) animatorCards2.SetTrigger("Start");
   }
   void ResolvePlayer(int result, ref bool playerFail, ref int[] cardsID, CardsPNG[] cardsUI)
   {
      if (result <= 0) { playerFail = true; return; }
      playerFail = false;

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
}
