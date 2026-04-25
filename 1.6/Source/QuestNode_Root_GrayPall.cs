using RimWorld;
using RimWorld.QuestGen;

namespace AnomalyRemixGrayPall
{
    public class QuestNode_Root_GrayPall : QuestNode
    {
        protected override void RunInt()
        {
            Quest quest = QuestGen.quest;
            QuestPart_LookTargets lookTargets = new QuestPart_LookTargets();
            quest.AddPart(lookTargets);
        }

        protected override bool TestRunInt(Slate slate) => true;
    }
}
