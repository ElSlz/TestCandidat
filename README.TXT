Cher(e) futur(e) collègue,

Votre mission, si vous l'acceptez, sera d'écrire le code nécéssaire à l'affichage d'un message dynamique après un temps fixé, en animant une barre de progression pour faire patienter l'utilisateur.

Les éléments clés : 
	une boite de texte (contenant "Toto" pour l'exemple)
	un slider permettant de rêgler la temporisation avant d'afficher le message	
	un slider permettant de rêgler la vitesse de raffraichissement de la barre de progression
	une barre de progression
	un bouton "GO!"
	un bouton "Stop"
	
Vous devrez donc complèter la classe "SpectacularViewModel"

Lors de l'appui sur le bouton "GO!"
	la barre de progression est remise à 0
	une temporisation se lance, la barre de progression se remplit.
	une fois le temps fixé passé, une pop-up apparait et affiche le texte de la boite de texte au moment de l'appui sur le bouton.

Lors de l'appui sur le bouton Stop : 
	si une temporisation est en cours, elle est stopée (et la barre de progression reste en l'état).
	si on modifie le slider de vitesse de raffraichissement pendant une temporisation, la vitesse de raffraichissement doit changer.

Contraintes générales: 
	le remplissage de la barre de progression doit être incrémental (on ne la remplit pas d'un coup à la fin de la temporisation)
	On ne touche pas aux fichiers qui sont dans le projet TestCandidatBase
	La temporisation doit être respectée à 30ms prêt.