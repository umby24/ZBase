﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ZBase.Common {
    public static class TaskScheduler {
        internal static readonly ConcurrentDictionary<string, TaskItem> Tasks = new ConcurrentDictionary<string, TaskItem>();

        public static string RegisterTask(string name, TaskItem item) {
            TaskItem outitem;

            if (Tasks.TryGetValue(name, out outitem)) {
                Logger.Log(LogType.Warning, $"Attempted to register already present task: {name}");
                name = name + new Random().Next(25, int.MaxValue);
                return RegisterTask(name, item);
            }

            Tasks.TryAdd(name, item);
            Logger.Log(LogType.Verbose, $"Registered task {name}");
            return name;
        }

        public static void UnregisterTask(string name) {
            TaskItem item;

            if (!Tasks.TryGetValue(name, out item)) {
                Logger.Log(LogType.Warning, $"Attempted to deregister non-existant task: {name}");
                return;
            }

            TaskItem what;
            Tasks.TryRemove(name, out what);
        }

        public static void RunSetupTasks() {
            foreach (KeyValuePair<string, TaskItem> taskItem in Tasks) {
                try {
                    taskItem.Value.Setup();
                } catch (Exception e) {
                    Logger.Log(LogType.Error, $"Error ocurred starting {taskItem.Key}: {e.Message}");
                    Logger.Log(LogType.Debug, $"Stacktrace: {e.StackTrace}");

                    if (e.InnerException == null)
                        continue;

                    Logger.Log (LogType.Error, $"Inner Error: {e.InnerException.Message}");
                    Logger.Log (LogType.Debug, $"Inner Stack: {e.InnerException.StackTrace}");
                }
            }
        }

        public static void RunMainTasks() {
            Watchdog.Watch("TaskScheduler", "Begin Mainloop..", true);
            foreach (KeyValuePair<string, TaskItem> taskItem in Tasks) {
                if ((DateTime.UtcNow - taskItem.Value.LastRun) < taskItem.Value.Interval)
                    continue;

                Watchdog.Watch("TaskScheduler", "Begin " + taskItem.Key, true);
                try {
                    taskItem.Value.Main();
                    taskItem.Value.LastRun = DateTime.UtcNow;
                } catch (Exception e) {
                    Logger.Log(LogType.Error, $"Error ocurred running {taskItem.Key}: {e.Message}");
                    Logger.Log(LogType.Debug, $"Stacktrace: {e.StackTrace}");
                    taskItem.Value.LastRun = DateTime.UtcNow;
                }
                Watchdog.Watch("TaskScheduler", "End " + taskItem.Key, true);
            }
            Watchdog.Watch("TaskScheduler", "End Mainloop", true);
        }

        public static void RunTeardownTasks() {
            foreach (KeyValuePair<string, TaskItem> taskItem in Tasks) {
                try {
                    taskItem.Value.Teardown();
                } catch (Exception e) {
                    Logger.Log(LogType.Error, $"Error ocurred tearing down {taskItem.Key}: {e.Message}");
                    Logger.Log(LogType.Debug, $"Stacktrace: {e.StackTrace}");
                }
            }
        }
    }
}
