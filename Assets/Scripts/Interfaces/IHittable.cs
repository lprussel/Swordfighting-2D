﻿using UnityEngine;
using System.Collections;

public interface IHittable
{
	void GotHit (PlayerManager attackingPlayer);
}