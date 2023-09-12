﻿using System;
using System.Collections.Generic;

namespace Chatter {
  public static class ListExtensions {
    public static List<T> Add<T>(this List<T> list, params T[] items) {
      list.AddRange(items);
      return list;
    }
  }
}
