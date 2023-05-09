using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class NavigationTargetStrategy : AIStrategy
{
    public ExpertSystemData expertSystemData;
    public override float CalculateApplicability(AIMind mind)
    {
        return 100;
    }

    public override void ApplyStrategy(ShipOrder order, AIMind mind)
    {
        //order.movementHasRotationDirection = true;
        if (mind.Perception.navigationTarget != null)
        {
            /*Vector3 moveVec = new Vector3(mind.Perception.navigationTarget.position.x, 0,
                mind.Perception.navigationTarget.position.y);
            if (Vector2.Angle(mind.Perception.navigationTarget.position-mind.Ship.transform.position, mind.Ship.transform.up) < 5)
            {
                moveVec = moveVec.MMSetY(1);
            }*/
            ///Угол между вектором напраления корабля и вектором до цели. Если он отрицательный, то цель левее, ели положительный
            /// То правее(или наоборот не помню:D)
            var angleToTarget =
                Vector2.SignedAngle(mind.Perception.navigationTarget.position - mind.Ship.transform.position,
                    mind.Ship.transform.up);
            var leftFeeler = mind.Ship.feelers[0];
            var middleFeeler = mind.Ship.feelers[1];
            var rightFeeler = mind.Ship.feelers[2];
            ///Расстояния до препятствий на соответствующих сенсорах
            var leftFeelerDistance = (leftFeeler.FeelTarget != null) ? leftFeeler.DistanceToTarget : 1000;
            var middleFeelerDistance = (middleFeeler.FeelTarget != null) ? middleFeeler.DistanceToTarget : 1000;
            var rightFeelerDistance = (rightFeeler.FeelTarget != null) ? rightFeeler.DistanceToTarget : 1000;
            ///Данные из настроек экспертной системы
            var inputVariables = expertSystemData.inputVariables;
            var outputVariables = expertSystemData.outputVariables;
            var rules = expertSystemData.rules;
             
            ///Алгоритм экспертной системы должен вызываться здесь!
            
            
            
            
            ///Дефаззифицированная скорость корабля
            float targetSpeed = 0;
            ///Дефазифицированная скорость поворота корабля
            float targetRotation = 0;
            ///Дефазифицированная горизонтальная скорость корабля, получаемая за счет работы боковых ускорителей 
            float targetHorizontalSpeed = 0;
            
            ///Здесь мы используя рассчитанные параметры отдаем приказ на движение кораблю
            var moveVec = new Vector3(Math.Sign(targetRotation),Math.Sign(targetSpeed),0);
            
            order.movement = moveVec;

            if (targetHorizontalSpeed > 0)
            {
                order.leftAdditionalMovement = true;
            }

            if (targetHorizontalSpeed < 0)
            {
                order.rightAdditionalMovement = true;
            }
        }
    }
}
