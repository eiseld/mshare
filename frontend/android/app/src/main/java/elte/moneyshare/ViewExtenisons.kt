package elte.moneyshare

import android.animation.AnimatorSet
import android.animation.ObjectAnimator
import android.animation.ValueAnimator
import android.app.Activity
import android.content.Context
import android.view.View
import android.view.animation.AccelerateDecelerateInterpolator
import android.view.inputmethod.InputMethodManager

const val ANIMATION_DURATION = 300L

fun View?.visible() {
    this?.visibility = View.VISIBLE
}

fun View?.invisible() {
    this?.visibility = View.INVISIBLE
}

fun View?.gone() {
    this?.visibility = View.GONE
}

fun View?.enable() {
    this?.isEnabled = true
}

fun View?.disable() {
    this?.isEnabled = false
}

fun View?.measure(): View {
    this?.measure(View.MeasureSpec.UNSPECIFIED, View.MeasureSpec.UNSPECIFIED)
    return this!!
}

/**
 * Works only on focusable views
 */
fun View?.showSoftKeyboard() {
    this?.requestFocus()
    val imm = this?.context?.getSystemService(Context.INPUT_METHOD_SERVICE) as InputMethodManager
    imm.showSoftInput(this, InputMethodManager.SHOW_IMPLICIT)
}


fun View?.hideSoftKeyboard() {
    val imm = this?.context?.getSystemService(Context.INPUT_METHOD_SERVICE)
    imm?.let {
        if ((this?.context as Activity).currentFocus != null) {
            (imm as InputMethodManager).hideSoftInputFromWindow(this.windowToken, 0)
        }
    }
}

fun View?.scaleUp(duration: Long = ANIMATION_DURATION) {
    if (this?.scaleX != 1F || this.scaleY != 1F) {
        val scaleUpX = ObjectAnimator.ofFloat(this, "scaleX", 0F, 1F)
        scaleUpX.duration = duration
        scaleUpX.interpolator = AccelerateDecelerateInterpolator()
        val scaleUpY = ObjectAnimator.ofFloat(this, "scaleY", 0F, 1F)
        scaleUpY.duration = duration
        scaleUpY.interpolator = AccelerateDecelerateInterpolator()
        val locationAnimatorSet = AnimatorSet()
        locationAnimatorSet.play(scaleUpX)
        locationAnimatorSet.play(scaleUpY)
        locationAnimatorSet.start()
    }
}

fun View?.scaleDown(duration: Long = ANIMATION_DURATION) {
    if (this?.scaleX != 0F || this.scaleY != 0F) {
        val scaleDownX = ObjectAnimator.ofFloat(this, "scaleX", 1F, 0F)
        scaleDownX.duration = duration
        scaleDownX.interpolator = AccelerateDecelerateInterpolator()
        val scaleDownY = ObjectAnimator.ofFloat(this, "scaleY", 1F, 0F)
        scaleDownY.duration = duration
        scaleDownY.interpolator = AccelerateDecelerateInterpolator()
        val locationAnimatorSet = AnimatorSet()
        locationAnimatorSet.play(scaleDownX)
        locationAnimatorSet.play(scaleDownY)
        locationAnimatorSet.start()
    }
}

fun View?.fadeIn(duration: Long = ANIMATION_DURATION) {
    val fadeIn = ObjectAnimator.ofFloat(this, "alpha", 0F, 1F)
    fadeIn.duration = duration
    fadeIn.interpolator = AccelerateDecelerateInterpolator()
    fadeIn.start()
}

fun View?.fadeOut(duration: Long = ANIMATION_DURATION) {
    val fadeOut = ObjectAnimator.ofFloat(this, "alpha", 1F, 0F)
    fadeOut.duration = duration
    fadeOut.interpolator = AccelerateDecelerateInterpolator()
    fadeOut.start()
}

fun View?.animateHeight(fromSize: Int, toSize: Int, endAnimationAction: (() -> Unit)? = null, duration: Long = ANIMATION_DURATION) {
    val animator = ValueAnimator.ofInt(fromSize, toSize).setDuration(duration)
    animator.interpolator = AccelerateDecelerateInterpolator()
    animator.addUpdateListener { animation ->
        this?.layoutParams?.height = animation.animatedValue as Int
        this?.requestLayout()
        if (animation.animatedValue == toSize) {
            this?.layoutParams?.height = toSize
            this?.requestLayout()
            endAnimationAction?.let { it() }
        }
    }
    animator.start()
}

fun View.fitWidthToScreen(context: Context) {
    this.layoutParams.width = context.resources.displayMetrics.widthPixels
}
