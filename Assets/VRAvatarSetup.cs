using UnityEngine;
using UnityEngine.XR;
using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Core;
using RootMotion.FinalIK;

public class VRAvatarSetup : MonoBehaviour
{
    public string AvatarURL = "URL_DEL_AVATAR_GENERADO_POR_READY_PLAYER_ME";
    public GameObject HeadTarget;
    public GameObject LeftHandTarget;
    public GameObject RightHandTarget;

    public Transform LeftController;
    public Transform RightController;

    private GameObject avatar;

    private void Start()
    {
        LoadAvatar(AvatarURL);
    }

    private void LoadAvatar(string avatarUrl)
    {
        var avatarLoader = new AvatarObjectLoader();
        avatarLoader.OnCompleted += (_, args) =>
        {
            avatar = args.Avatar;
            AvatarAnimatorHelper.SetupAnimator(args.Metadata.BodyType, avatar);
            SetupVRIK(avatar);
        };
        avatarLoader.LoadAvatar(avatarUrl);
    }

    private void SetupVRIK(GameObject avatar)
    {
        avatar.transform.SetParent(transform);
        avatar.transform.localPosition = Vector3.zero;
        avatar.transform.localRotation = Quaternion.identity;

        VRIK vrik = avatar.AddComponent<VRIK>();

        // Configurar las referencias VRIK
        VRIK.References references = new VRIK.References();
        references.root = avatar.transform;
        references.pelvis = FindTransform(avatar.transform, "Hips");
        references.spine = FindTransform(avatar.transform, "Spine");
        references.chest = FindTransform(avatar.transform, "Chest");
        references.neck = FindTransform(avatar.transform, "Neck");
        references.head = FindTransform(avatar.transform, "Head");
        references.leftShoulder = FindTransform(avatar.transform, "LeftShoulder");
        references.leftUpperArm = FindTransform(avatar.transform, "LeftUpperArm");
        references.leftForearm = FindTransform(avatar.transform, "LeftLowerArm");
        references.leftHand = FindTransform(avatar.transform, "LeftHand");
        references.rightShoulder = FindTransform(avatar.transform, "RightShoulder");
        references.rightUpperArm = FindTransform(avatar.transform, "RightUpperArm");
        references.rightForearm = FindTransform(avatar.transform, "RightLowerArm");
        references.rightHand = FindTransform(avatar.transform, "RightHand");
        references.leftThigh = FindTransform(avatar.transform, "LeftUpperLeg");
        references.leftCalf = FindTransform(avatar.transform, "LeftLowerLeg");
        references.leftFoot = FindTransform(avatar.transform, "LeftFoot");
        references.leftToes = FindTransform(avatar.transform, "LeftToes");
        references.rightThigh = FindTransform(avatar.transform, "RightUpperLeg");
        references.rightCalf = FindTransform(avatar.transform, "RightLowerLeg");
        references.rightFoot = FindTransform(avatar.transform, "RightFoot");
        references.rightToes = FindTransform(avatar.transform, "RightToes");

        vrik.references = references;

        // Configurar los objetivos para la locomoción
        vrik.solver.spine.headTarget = HeadTarget.transform;
        vrik.solver.leftArm.target = LeftHandTarget.transform;
        vrik.solver.rightArm.target = RightHandTarget.transform;

        // Asegurarse de que los duplicados sean hijos de los objetos correspondientes
        //HeadTarget.transform.SetParent(Camera.main.transform, false);
        //LeftHandTarget.transform.SetParent(LeftController, false);
        //RightHandTarget.transform.SetParent(RightController, false);

        // Inicializa VRIK
        vrik.AutoDetectReferences();
        vrik.solver.IKPositionWeight = 1f;
    }

    private Transform FindTransform(Transform parent, string name)
    {
        Transform result = parent.Find(name);
        if (result == null)
        {
            foreach (Transform child in parent)
            {
                result = FindTransform(child, name);
                if (result != null) break;
            }
        }
        return result;
    }

    private void OnDestroy()
    {
        if (avatar != null) Destroy(avatar);
    }
}
