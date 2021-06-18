package com.unity3d.player

import android.app.Activity
import android.app.Application
import android.util.Log
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import org.koin.android.ext.koin.androidContext
import org.koin.android.ext.koin.androidLogger
import org.koin.androidx.workmanager.koin.workManagerFactory
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject
import org.koin.core.logger.Level
import org.sagebionetworks.bridge.kmm.shared.cache.ResourceResult
import org.sagebionetworks.bridge.kmm.shared.di.initKoin
import org.sagebionetworks.bridge.kmm.shared.repo.AuthenticationRepository

class UnityKotlin: KoinComponent {
    companion object {
        private const val LOG_TAG = "UnityKotlin"
        private const val GameObjectStr = "BridgeClient"
    }

    private val auth: AuthenticationRepository by inject()

    constructor(app: Application) {
        initKoin (enableNetworkLogs = BuildConfig.DEBUG) {
            androidLogger(Level.ERROR)
            androidContext(app)
            workManagerFactory()
        }
        Log.d(LOG_TAG, "$LOG_TAG initKMM called")
    }

    fun signIn(externalId: String) {
//        if (auth.isAuthenticated()) {
//            // Send unity success function
//            Log.d(LOG_TAG, "$LOG_TAG auth already succeeded")
//            return
//        }

        val main = CoroutineScope(context = Dispatchers.Main)
        val scope = CoroutineScope(context = Dispatchers.IO)
        scope.launch {
            val result = auth.signInExternalId(externalId, externalId)
            main.launch {
                if (result is ResourceResult.Success && result.data.authenticated) {
                    // Send unity success function
                    Log.d(LOG_TAG, "$LOG_TAG auth succeeded")
                    UnityPlayer.UnitySendMessage(GameObjectStr, "signInComplete", "1234567")
                } else {
                    // Send unity failure function
                    Log.d(LOG_TAG, "$LOG_TAG auth failed")
                }
            }
        }
    }
}