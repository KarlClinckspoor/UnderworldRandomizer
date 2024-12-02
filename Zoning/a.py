import random
from enum import Enum, auto
import numpy as np
import matplotlib.pyplot as plt


class SkillCheckResult(Enum):
    CritSuccess = auto()
    Success = auto()
    Fail = auto()
    CritFail = auto()


def skill_check(value, target):
    score = (value - target) + random.randint(0, 31)
    if score <= 28:
        if score <= 15:
            if score <= 2:
                return SkillCheckResult.CritFail
            return SkillCheckResult.Fail
        return SkillCheckResult.Success
    return SkillCheckResult.CritSuccess

def skill_check_ratios(value, target):
    die_rolls = np.arange(0, 31)
    results = {SkillCheckResult.CritFail: 0, SkillCheckResult.Fail: 0, SkillCheckResult.Success: 0, SkillCheckResult.CritSuccess: 0}
    for roll in die_rolls:
        score = (value - target) + roll
        if score <= 28:
            if score <= 15:
                if score <= 2:
                    results[SkillCheckResult.CritFail] += 1
                    continue
                results[SkillCheckResult.Fail] += 1
                continue
            results[SkillCheckResult.Success] += 1
            continue
        else:
            results[SkillCheckResult.CritSuccess] += 1

    for key in results.keys():
        results[key] /= len(die_rolls)
    
    assert sum(results.values()) == 1
    return results
                

def main():
    tries = 1000
    difficulties = np.arange(1, 16)
    lockpick_skill = np.arange(0, 31)
    for dex in [15, 18, 20, 22, 25, 30]:
        map_success = np.zeros((len(difficulties), len(lockpick_skill)))
        map_break = np.zeros_like(map_success)
        for i in range(len(difficulties)):  # pylint: disable=consider-using-enumerate
            for j in range( len(lockpick_skill)):  # pylint: disable=consider-using-enumerate
                difficulty = difficulties[i]
                skill = lockpick_skill[j]
                successes = 0
                breaks = 0
                for _ in range(tries):
                    attempt = skill_check(skill + 1, difficulty * 3)
                    if (attempt == SkillCheckResult.CritSuccess) or ( attempt == SkillCheckResult.Success):
                        successes += 1
                        continue
                    if attempt == SkillCheckResult.CritFail:
                        break_ = skill_check(dex, 20)
                        if (break_ == SkillCheckResult.Fail) or ( break_ == SkillCheckResult.CritFail):
                            breaks += 1
                success_rate = successes / tries
                break_rate = breaks / tries
                map_success[i, j] = success_rate
                map_break[i, j] = break_rate

        plt.figure(dex)
        plt.imshow(map_success)
        plt.title(f"Success rate map {dex=}")
        plt.contour(map_break)
        plt.show()

def main2():
    difficulties = np.arange(1, 16)
    lockpick_skill = np.arange(1, 31)
    for dex in [30]:
        map_success = np.zeros((len(difficulties), len(lockpick_skill)))
        map_break = np.zeros_like(map_success)
        for i in range(len(difficulties)):  # pylint: disable=consider-using-enumerate
            for j in range( len(lockpick_skill)):  # pylint: disable=consider-using-enumerate
                difficulty = difficulties[i]
                skill = lockpick_skill[j]
                ratios_pick = skill_check_ratios(skill+1, difficulty*3)
                ratios_break = skill_check_ratios(dex, 20)
                success_rate = ratios_pick[SkillCheckResult.Success] + ratios_pick[SkillCheckResult.CritSuccess]
                break_rate = (ratios_break[SkillCheckResult.CritFail] + ratios_break[SkillCheckResult.Fail]) * ratios_pick[SkillCheckResult.CritFail] # Success and CritSuccess are ignored

                map_success[i, j] = success_rate
                map_break[i, j] = break_rate

        plt.figure(dex)
        # plt.imshow(map_success, extent=(lockpick_skill[0], lockpick_skill[-1], difficulties[-1], difficulties[0]  ))
        plt.imshow(map_success)
        plt.title(f"Success rate map {dex=}")
        # plt.contour(map_success, levels=np.arange(0, 1.1, 0.1), cmap='gray_r', extent=(lockpick_skill[0], lockpick_skill[-1], difficulties[-1], difficulties[0]  ), origin='image')
        plt.contour(map_success, levels=np.arange(0, 1.1, 0.1), cmap='gray_r')
        for i in range(map_break.shape[0]):
            for j in range(map_break.shape[1]):
                plt.text(j, i, f'{map_break[i, j]*100:.1f}', c='r', ha='center', va='center', fontsize=5)
                # plt.text(lockpick_skill[j], difficulties[i], f'{map_break[i, j]*100:.1f}', c='r', ha='center', va='center', fontsize=5)

        plt.xlabel('Lockpicking skill')
        plt.ylabel('Lock difficulty')
        plt.xticks([0, 4, 9, 14, 19, 24, 29], ['1', '5', '10', '15', '20', '25', '30'])
        plt.yticks([0, 4, 9, 14], [1, 5, 10, 15])

        plt.axhline(4, c='r')
        plt.axhline(14, c='magenta')

    plt.show()


if __name__ == "__main__":
    main2()
